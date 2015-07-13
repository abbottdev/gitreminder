using Git.Reminder.Models;
using Git.Reminder.Providers.Storage.Credentials;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Git.Reminder.Providers
{
    public class CredentialProvider
    {
        public readonly CredentialsHandler CredentialHandler;
        private ICredentialStore store;

        private Lazy<IEnumerable<CredentialModel>> credentials;

        public CredentialProvider(IScreen screen, ICredentialStore credentialStore)
        {
            this.store = credentialStore;
            this.CredentialHandler = new CredentialsHandler(this.OnCredentialsRequested);

        }

        private Credentials OnCredentialsRequested(string forUrl, string usernameFromUrl, SupportedCredentialTypes types)
        {
            if (types == SupportedCredentialTypes.Default)
            {
                return new DefaultCredentials();
            }
            else
            {
                var credentials = Task.Run(() => store.GetCredentialsAsync()).Result;

                if (credentials != null)
                {
                    var credential = credentials.Where(c => c.Url == forUrl && (string.IsNullOrEmpty(usernameFromUrl) ? true : c.UserName == usernameFromUrl)).SingleOrDefault();

                    if (credential != null)
                    {
                        return new UsernamePasswordCredentials()
                        {
                            Username = credential.UserName,
                            Password = credential.Password
                        };
                    }
                }

                if (types == SupportedCredentialTypes.Default)
                {
                    return new DefaultCredentials();
                }
                else
                {
                    return new UsernamePasswordCredentials();
                }
            }
        }
    }
}
