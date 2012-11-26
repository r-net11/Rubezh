using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using Infrastructure.Common.BalloonTrayTip.ViewModels;

namespace Infrastructure.Common.BalloonTrayTip.Views
{
    public partial class CustomBalloonView : UserControl
    {
        bool isClosing = false;
        
        public CustomBalloonView()
        {
            InitializeComponent();
            TaskbarIcon.AddBalloonClosingHandler(this, OnBalloonClosing);
        }

        private void grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.ResetBalloonCloseTimer();
            if (BalloonToolTipViewModel.isEmpty)
            {
                taskbarIcon.CloseBalloon();
                taskbarIcon.Dispose();
            }
        }

        private void OnBalloonClosing(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            isClosing = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.ResetBalloonCloseTimer();
            if (BalloonToolTipViewModel.isEmpty)
            {
                taskbarIcon.CloseBalloon();
                taskbarIcon.Dispose();
            }
        }
    }

    public class UnstyledButton : Button
    {
        
    }
}
