using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
                MyNotifyIcon.ShowCustomBalloon(customBalloonView, System.Windows.Controls.Primitives.PopupAnimation.Scroll, 4000);
            }
            else
                MyNotifyIcon.ShowBalloonTip(BalloonToolTipViewModel.BalloonTitle, BalloonToolTipViewModel.BalloonText, MyNotifyIcon.Icon);
            
        }
    }
}

//xmlns:tb="http://www.hardcodet.net/taskbar"