using Git.Reminder.ViewModels;
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
    /// Interaction logic for CurrentRepositoryStatusView.xaml
    /// </summary>
    public partial class CurrentRepositoryStatusView : UserControl, IViewFor<CurrentRepositoryStatusViewModel>
    {
        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(CurrentRepositoryStatusViewModel), typeof(CurrentRepositoryStatusView), new UIPropertyMetadata(null));


        public CurrentRepositoryStatusViewModel ViewModel
        {
            get { return (CurrentRepositoryStatusViewModel)GetValue(ViewModelProperty); }
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
                ViewModel = (CurrentRepositoryStatusViewModel)value;
            }
        }

        public CurrentRepositoryStatusView()
        {
            InitializeComponent();
        }
         
    }
}
