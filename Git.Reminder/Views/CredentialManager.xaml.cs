using Git.Reminder.ViewModels.Credentials;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Git.Reminder.Views
{
    /// <summary>
    /// Interaction logic for CredentialManager.xaml
    /// </summary>
    public partial class CredentialManager : UserControl, IViewFor<CredentialManagerViewModel>
    {

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(CredentialManagerViewModel), typeof(CredentialManager), new UIPropertyMetadata(null));

        public CredentialManagerViewModel ViewModel
        {
            get { return (CredentialManagerViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DataContext = ViewModel;
        }

        object IViewFor.ViewModel
        {
            get
            {
                return ViewModel;
            }
            set
            {
                ViewModel = (CredentialManagerViewModel)value;
            }
        }


        public CredentialManager()
        {
            InitializeComponent();
        }
    }
}
