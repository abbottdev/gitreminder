using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using param = System.Windows.SystemParameters;

namespace Git.Reminder
{
    /// <summary>
    /// Interaction logic for ReminderWindow.xaml
    /// </summary>
    public partial class ReminderWindow : Window
    {
        public ReminderWindow()
        {
            InitializeComponent();

            this.MouseDown += ReminderWindow_MouseDown;
            this.Loaded += ReminderWindow_Loaded;
            this.Closing += ReminderWindow_Closing;

            var mouseMoves = Observable.FromEvent<MouseEventHandler, MouseEventArgs>(
                (add) => this.MouseMove += add,
                (rem) => this.MouseMove -= rem
            );

            var mouseEnters = Observable.FromEvent<MouseEventHandler, MouseEventArgs>(
                (add) => this.MouseEnter += add,
                (rem) => this.MouseEnter -= rem
            );

            var mouseLeaves = Observable.FromEvent<MouseEventHandler, MouseEventArgs>(
                (add) => this.MouseLeave += add,
                (rem) => this.MouseLeave -= rem
                );



        }

        void ReminderWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SavePosition();
        }

        private void SavePosition()
        {
            var settings = Git.Reminder.Properties.Settings.Default;

            settings.ReminderWindow_LastPosition = new Point(this.Left, this.Top);

            settings.Save();
        }


        void ReminderWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPositionFromStorage();
        }

        private void LoadPositionFromStorage()
        {
            Point lastPosition = Git.Reminder.Properties.Settings.Default.ReminderWindow_LastPosition;

            if (lastPosition.X != 0 && lastPosition.Y != 0 && VirtualScreenContainsPosition(lastPosition))
            {
                this.Left = lastPosition.X;
                this.Top = lastPosition.Y;
            }
            else
            {
                this.Left = param.WorkArea.Left + param.WorkArea.Width - this.ActualWidth - 50;
                this.Top = param.WorkArea.BottomRight.Y - this.ActualHeight - 10;

            }
        }

        private bool VirtualScreenContainsPosition(Point lastPosition)
        {
            Rect r = new Rect(param.VirtualScreenLeft, param.VirtualScreenTop, param.VirtualScreenWidth, param.VirtualScreenHeight);

            return r.Contains(lastPosition);
        }

        void ReminderWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            {
                try
                {
                    DragMove();
                }
                catch { }
            }
        }


    }
}
