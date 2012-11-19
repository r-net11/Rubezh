using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Infrastructure.Common.BalloonTrayTip.ViewModels;

namespace Infrastructure.Common.BalloonTrayTip.Views
{
    public partial class BalloonToolTipView : UserControl
    {
        public BalloonToolTipView()
        {
            InitializeComponent();
        }

        private void MyNotifyIcon_TrayBalloonTipClosed(object sender, RoutedEventArgs e)
        {
            MyNotifyIcon.Dispose();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (BalloonToolTipViewModel.IsCustom)
            {
                CustomBalloonView customBalloonView = new CustomBalloonView();
                MyNotifyIcon.ShowCustomBalloon(customBalloonView, System.Windows.Controls.Primitives.PopupAnimation.None, 4000);
            }
            else
                MyNotifyIcon.ShowBalloonTip(BalloonToolTipViewModel.BalloonTitle, BalloonToolTipViewModel.BalloonText, MyNotifyIcon.Icon);
            
        }

        private void MyNotifyIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MyNotifyIcon.Dispose();
        }
    }
}

//xmlns:tb="http://www.hardcodet.net/taskbar"