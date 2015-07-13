using Git.Reminder.Providers.Storage.Credentials;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Git.Reminder.ViewModels
{
    public class AppBootstrapper : IScreen
    {

        RootViewModel rootViewModel;
        private RoutingState router;

        public RootViewModel Root
        {
            get
            {
                return this.rootViewModel;
            }
        }

        public AppBootstrapper(ICredentialStore store)
        {
            //ReactiveUI.ViewLocator.Current
            this.router = new RoutingState();
            this.rootViewModel = new RootViewModel(this, store);
        }


        #region IScreen Members

        public RoutingState Router
        {
            get { return this.router; }
        }

        #endregion
    }
}
