using Git.Reminder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Git.Reminder.Providers.Storage.Credentials
{
    public interface ICredentialStore
    {
        /// <summary>
        /// Loads the credentials from storage
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CredentialModel>> GetCredentialsAsync();

        /// <summary>
        /// Saves the credentials to storage
        /// </summary>
        /// <param name="credentials">The credentials to store</param>
        /// <returns></returns>
        Task SaveCredentialsAsync(IEnumerable<CredentialModel> credentials);
    }
}
