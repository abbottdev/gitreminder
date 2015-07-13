using Git.Reminder.Models;
using Git.Reminder.Providers.Storage.Credentials;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Git.Reminder.ViewModels.Credentials
{
    public class CredentialManagerViewModel : ReactiveObject, ICredentialManagerViewModel
    {
        private ReactiveList<CredentialModel> credentialList;
        private ReactiveCommand<IEnumerable<CredentialModel>> loadCredentials;
        private IScreen screen;

        /// <summary>
        /// The credentials list
        /// </summary>
        public ReactiveList<CredentialModel> Credentials
        {
            get
            {
                return this.credentialList;
            }
        }

        public CredentialManagerViewModel(IScreen screen, ICredentialStore credentialStore)
        {
            this.screen = screen;
            ConfigureCredentialPersistence(credentialStore);
        }

        /// <summary>
        /// Configures credentials properties and auto persistence from the credential store.
        /// </summary>
        /// <param name="credentialStore">The <see cref="ICredentialStore">ICredentialStore</see> used to persist credentials.</param>
        private void ConfigureCredentialPersistence(ICredentialStore credentialStore)
        {
            var loadCredentialsTask = Task.Run<IEnumerable<CredentialModel>>(async () =>
            {
                return await credentialStore.GetCredentialsAsync().ConfigureAwait(false); 
            });


            this.credentialList = new ReactiveList<CredentialModel>(loadCredentialsTask.Result);

            this.credentialList.AutoPersist((creds) =>
            {
                return Observable.FromAsync<Unit>(async () =>
                {
                    await credentialStore.SaveCredentialsAsync(creds.AsEnumerable());
                    return Unit.Default;
                });
            });
        }

        #region IRoutableViewModel Members

        public IScreen HostScreen
        {
            get
            {
                return this.screen;
            }
        }

        public string UrlPathSegment
        {
            get
            {
                return "/credentials/";
            }
        }

        #endregion
    }
}
