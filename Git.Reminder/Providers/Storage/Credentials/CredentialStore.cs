using Akavache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace Git.Reminder.Providers.Storage.Credentials
{
    class CredentialStore : ICredentialStore
    {
        
        #region ICredentialStore Members

        public async Task<IEnumerable<Models.CredentialModel>> GetCredentialsAsync()
        {
            return await BlobCache
                .Secure
                .GetOrCreateObject<IEnumerable<Models.CredentialModel>>("credentials", () => new List<Models.CredentialModel>());
        }

        public async Task SaveCredentialsAsync(IEnumerable<Models.CredentialModel> credentials)
        {
            await BlobCache.Secure.InsertObject<IEnumerable<Models.CredentialModel>>("credentials", credentials);
        }

        #endregion
    }
}
