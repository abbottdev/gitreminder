using Akavache;
using Git.Reminder.Providers;
using Git.Reminder.Providers.Storage;
using Git.Reminder.Providers.Storage.Credentials;
using Git.Reminder.ViewModels;
using Git.Reminder.ViewModels.Credentials;
using Ninject;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Splat;

namespace Git.Reminder
{
    
    internal sealed class DependencyRegistration
    {
        public static void Register()
        {
            BlobCache.ApplicationName = "GitReminder";

            Locator.CurrentMutable.Register(() => new JsonNetSerializer(), typeof(ISerializer));

            Locator.CurrentMutable.Bind<ISerializer, JsonNetSerializer>();
            Locator.CurrentMutable.Bind<ICredentialStore, CredentialStore>();

            Locator.CurrentMutable.BindWithConstructors<ICredentialManagerViewModel, CredentialManagerViewModel>();
            //resolver.Bind<ICredentialManagerViewModel, CredentialManagerViewModel>();

            Locator.CurrentMutable.Bind<IViewFor<CredentialManagerViewModel>, Views.CredentialManager>();
            Locator.CurrentMutable.Bind<IViewFor<CurrentRepositoryStatusViewModel>, Views.CurrentRepositoryStatusView>();

            //Locator.Current = new NinjectResolver(kernel);

            //Locator.CurrentMutable.InitializeSplat();
            //Locator.CurrentMutable.InitializeReactiveUI();
            
        }

    }
}
