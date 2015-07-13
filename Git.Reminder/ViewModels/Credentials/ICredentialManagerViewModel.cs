using Git.Reminder.Models;
using ReactiveUI;
using System;
namespace Git.Reminder.ViewModels.Credentials
{
  public  interface ICredentialManagerViewModel : IRoutableViewModel
    {
        ReactiveList<CredentialModel> Credentials { get; }
    }
}
