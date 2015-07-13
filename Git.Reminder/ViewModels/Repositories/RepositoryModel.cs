using LibGit2Sharp;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace Git.Reminder.ViewModels
{
    public class RepositoryModel : ReactiveObject
    {
        private string path;
        private ObservableAsPropertyHelper<string> projectName;
        private ObservableAsPropertyHelper<IRepository> repository;
        private ObservableAsPropertyHelper<Branch> activeBranch;
        private ObservableAsPropertyHelper<DirectoryInfo> workingFolder;

        public IRepository Repository
        {
            get
            {
                return this.repository.Value;
            }
        }

        public DirectoryInfo WorkingFolder
        {
            get
            {
                return this.workingFolder.Value;
            }
        }

        public string Path
        {
            get { return path; }
            set { this.RaiseAndSetIfChanged(ref this.path, value); }
        }

        public string ProjectName
        {
            get
            {
                return this.projectName.Value;
            }
        }

        private static Branch GetCurrentTrackedBranchFor(IRepository repository)
        {
            return repository.Branches.Where(b => b.IsCurrentRepositoryHead).SingleOrDefault();
        }

        public Branch CurrentBranch
        {
            get
            {
                return this.activeBranch.Value;
            }
        }

        public RepositoryModel(string path)
            : this()
        {
            if (Directory.Exists(path) == false)
            {
                throw new ArgumentException("This path/folder does not exist");
            }

            this.Path = path;
        }

        public RepositoryModel()
        {

            var path = this
                .WhenAny(vm => vm.Path, change => change.GetValue());

            //Setup a working folder so we can reference anything we need to.
            path
                .Select(p => new DirectoryInfo(System.IO.Path.GetDirectoryName(p)))
                .ToProperty(this, vm => vm.WorkingFolder, out this.workingFolder, null, Scheduler.Immediate);

            //Configure the project name (initially) to be the folder name
            this
                .WhenAny(vm => vm.WorkingFolder, change => change.GetValue().Name)
                .ToProperty(this, vm => vm.ProjectName, out this.projectName, null, Scheduler.Immediate);

            //Create repository property
            path.Select(p =>
            {
                return (IRepository)new Repository(p);
            })
            .ToProperty(this, vm => vm.Repository, out this.repository, null, Scheduler.Immediate);

            //Setup current branch tracker
            this
                .WhenAny(vm => vm.Repository, change => change.GetValue()).Where(r => r != null)
                .Select(repo => GetCurrentTrackedBranchFor(repo))
                .ToProperty(this, vm => vm.CurrentBranch, out this.activeBranch, null, Scheduler.Immediate);

        }


    }
}
