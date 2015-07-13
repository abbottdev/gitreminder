using LibGit2Sharp;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive;

namespace Git.Reminder.ViewModels
{
    public class IndexViewModel : ReactiveObject
    {
        private IRepository repo;
        private ReactiveCommand<object> refreshCommand;
        private ObservableAsPropertyHelper<RepositoryStatus> repositoryStatus;
        private ObservableAsPropertyHelper<IEnumerable<StatusEntry>> statusEntries;
        private ObservableAsPropertyHelper<IReactiveDerivedList<StatusEntry>> unstagedEntries;
        private ObservableAsPropertyHelper<IReactiveDerivedList<StatusEntry>> stagedEntries;

        public IReactiveDerivedList<StatusEntry> UnstagedEntries
        {
            get
            {
                return this.unstagedEntries.Value;
            }
        }

        public IReactiveDerivedList<StatusEntry> StagedEntries
        {
            get
            {
                return this.stagedEntries.Value;
            }
        }

        public IEnumerable<StatusEntry> StatusEntries
        {
            get
            {
                return this.statusEntries.Value;
            }
        }

        public RepositoryStatus RepositoryStatus
        {
            get
            {
                return this.repositoryStatus.Value;
            }
        }

        public IndexViewModel(IRepository repo)
        {
            this.repo = repo;
            this.refreshCommand = ReactiveCommand.Create();

            this.repositoryStatus = this.refreshCommand.Select(u =>
            {
                return repo.RetrieveStatus(new StatusOptions() { Show = StatusShowOption.IndexAndWorkDir });
            }).ToProperty(this, vm => vm.RepositoryStatus);

            this.statusEntries = this
                .WhenAny(vm => vm.RepositoryStatus, change =>
                {
                    var status = change.GetValue();

                    return status.CreateDerivedCollection(s => s, null, null, null, this.refreshCommand);
                }).ToProperty(this, vm => vm.StatusEntries);

            var resetSignal = this.WhenAny(vm => vm.StatusEntries, change =>
             {
                 return 0;
             });

            var allEntries = this.WhenAny(vm => vm.StatusEntries, change => change.GetValue());

            this.unstagedEntries = allEntries.Select(s =>
            {
                return s.CreateDerivedCollection(i => i, i => Unstaged(i.State), null, resetSignal);
            }).ToProperty(this, vm => vm.UnstagedEntries);


            this.stagedEntries = allEntries.Select(s =>
            {
                return s.CreateDerivedCollection(i => i, i => Staged(i.State), null, resetSignal);
            }).ToProperty(this, vm => vm.StagedEntries);

        }

        private bool Unstaged(FileStatus status)
        {
            return !Staged(status);
        }

        private bool Staged(FileStatus status)
        {
            switch (status)
            {
                case FileStatus.NewInIndex:
                case FileStatus.ModifiedInIndex:
                case FileStatus.TypeChangeInIndex:
                case FileStatus.RenamedInIndex:
                case FileStatus.DeletedFromIndex:
                    return true;
                default:
                    return false;
            }
        }
    }
}
