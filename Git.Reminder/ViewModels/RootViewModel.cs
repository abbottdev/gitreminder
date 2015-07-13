using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Git.Reminder.Models;
using LibGit2Sharp;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.IO;
using Git.Reminder.Providers.Storage.Credentials;

namespace Git.Reminder.ViewModels
{
    public class RootViewModel : ReactiveObject, IDisposable 
    {
        private ReactiveList<RepositoryModel> repositories;
        private CurrentRepositoryStatusViewModel statusViewModel; 
        
        public CurrentRepositoryStatusViewModel Status
        {
            get
            {
                return this.statusViewModel;
            }
        }

        public void Bookmark(string path)
        {
            this.repositories.Add(new RepositoryModel(path));
        }

        public void SelectRepositoryWithPath(string path)
        {
            this.Status.ActiveRepository = this.repositories.Where(r => r.Path == path).Single();
        }

        public RootViewModel(IScreen screen, ICredentialStore store)
        { 
            this.repositories = new ReactiveList<RepositoryModel>();
            this.statusViewModel = new CurrentRepositoryStatusViewModel(screen, store); 
        }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
