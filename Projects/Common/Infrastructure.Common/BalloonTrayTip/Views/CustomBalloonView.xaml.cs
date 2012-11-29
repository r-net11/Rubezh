using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using Infrastructure.Common.BalloonTrayTip.ViewModels;

namespace Infrastructure.Common.BalloonTrayTip.Views
{
    public partial class CustomBalloonView : UserControl
    {
        public CustomBalloonView()
        {
            InitializeComponent();
            TaskbarIcon.AddBalloonClosingHandler(this, OnBalloonClosing);
        }

        private void grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (BalloonToolTipViewModel.isEmpty)
            {
                TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
                taskbarIcon.CloseBalloon();
                taskbarIcon.Dispose();
            }   
        }

        private void OnBalloonClosing(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            BalloonToolTipViewModel.isShown = false;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
            taskbarIcon.Dispose();
        }
    }
}
