using Git.Reminder.Providers.Storage.Credentials;
using Git.Reminder.ViewModels;
using Git.Reminder.ViewModels.Credentials;
using Ninject;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Git.Reminder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        public void OnStarted(object sender, StartupEventArgs e)
        {

            DependencyRegistration.Register();

            AppBootstrapper bootstrapper = new AppBootstrapper(Locator.Current.GetService<ICredentialStore>());

            Locator.CurrentMutable.RegisterConstant(bootstrapper, typeof(IScreen));



            bootstrapper.Root.Bookmark(@"E:\Repos\SFDC\");

            bootstrapper.Root.SelectRepositoryWithPath(@"E:\Repos\SFDC\");

            bootstrapper.Router.Navigate.Execute(Locator.Current.GetService<ICredentialManagerViewModel>());

            //bootstrapper.Router.Navigate.Execute(bootstrapper.Root.Status);

            MainWindow mainWindow = new MainWindow(bootstrapper);

            ReminderWindow reminderWindow = new ReminderWindow() { DataContext = bootstrapper.Root.Status };

            reminderWindow.Show();

            //var app = new App();

            mainWindow.Show();

            mainWindow.Closed += (sender2, e2) =>
            {
                reminderWindow.Close();
                Application.Current.Shutdown();
            };


           // reminderWindow.Close();
        }
         
    }
}
