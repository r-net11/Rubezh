using System.Windows;
using System.Windows.Controls;

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
            taskbarIcon.ShowCustomBalloon(customBalloonView, System.Windows.Controls.Primitives.PopupAnimation.None, 40000);
        }

        private void taskbarIcon_Unloaded(object sender, RoutedEventArgs e)
        {
            taskbarIcon.CloseBalloon();
            taskbarIcon.Dispose();
        }
    }
}

//xmlns:tb="http://www.hardcodet.net/taskbar"