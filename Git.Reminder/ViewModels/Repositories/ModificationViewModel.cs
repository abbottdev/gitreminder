using Git.Reminder.Models;
using LibGit2Sharp;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Git.Reminder.Providers;
using Git.Reminder.Providers.Storage.Credentials;

namespace Git.Reminder.ViewModels
{
    public class ModificationViewModel : ReactiveObject, IDisposable
    {
        private DeltaViewModel<int> files;
        private FileSystemObservable fileSystem;
        private ReactiveCommand<CommitResultModel> commitUntrackedChanges;
        private ReactiveCommand<CommitResultModel> commitTrackedChanges;
        private ObservableAsPropertyHelper<int> linesAdded;
        private ObservableAsPropertyHelper<int> linesRemoved;
        private RepositoryModel currentRepositoryModel;
        private Thresholds thresholds;
        private List<IDisposable> disposables;
        private ReactiveCommand<object> refreshCommand;
        private int commitsBehindBy;
        private int commitAheadBy;

        public Thresholds Thresholds
        {
            get
            {
                return this.thresholds;
            }
        }

        public int Behind
        {
            get
            {
                return commitsBehindBy;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref this.commitsBehindBy, value);
            }
        }

        public int Ahead
        {
            get
            {
                return commitAheadBy;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref this.commitAheadBy, value);
            }
        }

        public int LinesAdded
        {
            get
            {
                return this.linesAdded.Value;
            }
        }

        public int LinesRemoved
        {
            get
            {
                return this.linesRemoved.Value;
            }
        }

        public DeltaViewModel<int> Files
        {
            get
            {
                return this.files;
            }
        }

        public ICommand RefreshStatusCommand
        {
            get { return (ICommand)this.refreshCommand; }
        }

        public ModificationViewModel(IScreen screen, ICredentialStore store, IObservable<RepositoryModel> repository)
        {
            this.disposables = new List<IDisposable>();
            this.fileSystem = new FileSystemObservable();

            this.refreshCommand = ReactiveCommand.Create();


            var samples =
                Observable.Interval(TimeSpan.FromSeconds(60))
                .Select(i => new object())
                .Merge(this.refreshCommand.Select(i => new object()))
                .Merge(repository.Delay(TimeSpan.FromSeconds(1)).Select(i => new object()));

            var distinctRepository = repository.DistinctUntilChanged(r => r.Path);

            distinctRepository.MapToMember(this, vm => vm.currentRepositoryModel);
            distinctRepository.Select(_ => _.Path).MapToMember(this, vm => vm.fileSystem.Path);

            var activeBranch = repository
                .SelectMany(rm => rm.WhenAny(vm => vm.CurrentBranch, change => change.GetValue()))
                .SampleEx(samples);

            activeBranch
                .ObserveOn(System.Reactive.Concurrency.TaskPoolScheduler.Default)
                .SubscribeOn(System.Reactive.Concurrency.TaskPoolScheduler.Default)
                .Subscribe(branch =>
                {
                    if (branch.IsTracking)
                    {
                        //Fetch repository status.
                        var currentRemote = branch.Remote;

                        var refSpecs = this.currentRepositoryModel.Repository.Network.Remotes.Select(r =>
                                    new
                                    {
                                        FetchRefSpecs = r.FetchRefSpecs
                                                            .Where(frs => frs.Direction == RefSpecDirection.Fetch)
                                                            .Select(frs => frs.Specification),
                                        Remote = r
                                    }
                                );
                        var credentialProvider = new CredentialProvider(screen, store);

                        foreach (var item in refSpecs)
                        {
                            FetchOptions options = new FetchOptions() { CredentialsProvider = credentialProvider.CredentialHandler };
                            try
                            {
                                this.currentRepositoryModel.Repository.Network.Fetch(item.Remote, item.FetchRefSpecs, options);
                            }
                            catch { }
                        }
                    }
                });

            activeBranch
                .Select(b => b.TrackingDetails).Subscribe(_ =>
                {
                    this.Ahead = (_.AheadBy.HasValue) ? _.AheadBy.Value : 0;
                    this.Behind = (_.BehindBy.HasValue) ? _.BehindBy.Value : 0;
                });

            var activeBranchFileStatus =
                activeBranch.AnonymousMerge(fileSystem, 0)
                .AnonymousMerge(this.refreshCommand.AsObservable(), 0)
                .Throttle(TimeSpan.FromSeconds(5))
                .Select(branch =>
                {
                    var opts = new LibGit2Sharp.StatusOptions();

                    opts.DetectRenamesInIndex = true;
                    opts.DetectRenamesInWorkDir = true;
                    opts.Show = StatusShowOption.IndexAndWorkDir;

                    return this.currentRepositoryModel.Repository.RetrieveStatus(opts);
                });

            this.files = new DeltaViewModel<int>
            (
                activeBranchFileStatus.Select(s => s.Modified.Count()),
                activeBranchFileStatus.Select(s => s.Added.Count() + s.Untracked.Count()),
                activeBranchFileStatus.Select(s => s.Removed.Count())
            );

            ConfigureLineChanges(activeBranch);

            ConfigureCommitCommands();


            this.thresholds = new Thresholds(
                this.ObservableForProperty(vm => vm.Behind, false, false).Select(change => change.GetValue()),
                this.ObservableForProperty(vm => vm.Ahead).Select(change => change.GetValue()),
                this.ObservableForProperty(vm => vm.LinesAdded).Select(change => change.GetValue()),
                this.ObservableForProperty(vm => vm.LinesRemoved).Select(change => change.GetValue()),
                this.Files.ObservableForProperty(vm => vm.Added).Select(change => change.GetValue()),
                this.Files.ObservableForProperty(vm => vm.Removed).Select(change => change.GetValue()));

        }

        private void ConfigureLineChanges(IObservable<Branch> activeBranch)
        {

            var lineChanges = activeBranch.Merge(fileSystem.ThrottleFirst(TimeSpan.FromSeconds(15)).Select(c => this.currentRepositoryModel.CurrentBranch)).Select(b =>
            {
                Commit lastBranchCommit = b.Tip;
                CompareOptions options = new CompareOptions()
                {
                    IncludeUnmodified = false,
                    ContextLines = 5,
                    InterhunkLines = 5
                };

                var repo = this.currentRepositoryModel.Repository;
                //CommitFilter filter = new CommitFilter() { Since = b.Tip.Sha };

                var patch = repo.Diff.Compare<Patch>(repo.Head.Tip.Tree, DiffTargets.WorkingDirectory);

                return new { Added = patch.Sum(p => p.LinesAdded), Removed = patch.Sum(p => p.LinesDeleted) };
            });

            this.linesAdded = lineChanges.Select(c => c.Added).ToProperty(this, a => a.LinesAdded, 0, Scheduler.Immediate);
            this.linesRemoved = lineChanges.Select(c => c.Removed).ToProperty(this, a => a.LinesRemoved, 0, Scheduler.Immediate);
        }

        private void ConfigureCommitCommands()
        {

            var canCommit = this.Files.WhenAny(vm => vm.Modified, vm => vm.Added, vm => vm.Removed, (mod, add, rem) =>
            {
                return mod.GetValue() > 0 || add.GetValue() > 0 || rem.GetValue() > 0;
            });

            this.commitUntrackedChanges = ReactiveCommand.CreateAsyncTask<CommitResultModel>(canCommit, async x =>
            {
                throw new NotImplementedException();
            });


            this.commitTrackedChanges = ReactiveCommand.CreateAsyncTask<CommitResultModel>(canCommit, async x =>
            {
                //Commit every modified file (tracked)
                StatusOptions status;
                StageOptions stage;
                IRepository repo;
                IList<string> committedFiles;
                IEnumerable<StatusEntry> modifiedFiles;

                committedFiles = new List<string>();
                stage = new StageOptions();
                status = new StatusOptions()
                {
                    DetectRenamesInIndex = true,
                    DetectRenamesInWorkDir = true
                };

                repo = this.currentRepositoryModel.Repository;

                modifiedFiles = await Task.Run<IEnumerable<StatusEntry>>(() => repo.RetrieveStatus(status).Modified);

                //Now stage all modified files
                foreach (var file in modifiedFiles)
                {
                    if (file.State.HasFlag(FileStatus.ModifiedInWorkdir) && file.State.HasFlag(FileStatus.ModifiedInIndex) == false)
                    {
                        await Task.Run(() => repo.Stage(file.FilePath, stage));
                        committedFiles.Add(file.FilePath);
                    }
                }

                return new CommitResultModel() { Files = committedFiles.AsEnumerable() };
            });
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (var item in disposables)
            {
                item.Dispose();
            }
        }

        #endregion
    }
}
