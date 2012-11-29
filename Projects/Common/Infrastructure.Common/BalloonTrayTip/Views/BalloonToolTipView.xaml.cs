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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CustomBalloonView customBalloonView = new CustomBalloonView();
            MyNotifyIcon.ShowCustomBalloon(customBalloonView, System.Windows.Controls.Primitives.PopupAnimation.None, 40000);
        }
    }
}

//xmlns:tb="http://www.hardcodet.net/taskbar"