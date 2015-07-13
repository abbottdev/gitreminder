using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Git.Reminder.Providers.Storage.Credentials;

namespace Git.Reminder.ViewModels
{
    public class CurrentRepositoryStatusViewModel : ReactiveObject, IRoutableViewModel
    {
        private RepositoryModel repository;
        private ModificationViewModel modifications;
        private IScreen screen;

        public ModificationViewModel Modifications
        {
            get
            {
                return this.modifications;
            }
        }

        public CurrentRepositoryStatusViewModel(IScreen screen, ICredentialStore store)
        {
            this.screen = screen;
            var activeRepository = this.WhenAny(vm => vm.ActiveRepository, change => change.GetValue()).Where(r => r != null).DistinctUntilChanged(r => r.Path);
            this.modifications = new ModificationViewModel(screen, store, activeRepository);
        }

        public RepositoryModel ActiveRepository
        {
            get
            {
                return this.repository;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref this.repository, value);
            }
        }

        #region IRoutableViewModel Members

        public IScreen HostScreen
        {
            get { return this.screen; }
        }

        public string UrlPathSegment
        {
            get { return "status/"; }
        }

        #endregion
    }
}
