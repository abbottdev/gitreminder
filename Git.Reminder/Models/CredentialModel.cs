using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Reactive.Linq;

namespace Git.Reminder.Models
{
    public class CredentialModel : ReactiveObject
    {
        private string url;
        private string userName;
        private string password;

        public string Password
        {
            get { return password; }
            set { this.RaiseAndSetIfChanged(ref this.password, value); }
        }

        public string UserName
        {
            get { return userName; }
            set { this.RaiseAndSetIfChanged(ref this.userName, value); }
        }

        public string Url
        {
            get { return url; }
            set { this.RaiseAndSetIfChanged(ref this.url, value); }
        }


    }
}
