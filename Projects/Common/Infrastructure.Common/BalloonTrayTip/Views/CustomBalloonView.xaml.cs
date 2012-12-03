using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using Infrastructure.Common.BalloonTrayTip.ViewModels;
using System.Windows.Media.Imaging;
using System;

namespace Infrastructure.Common.BalloonTrayTip.Views
{
    public partial class CustomBalloonView : UserControl
    {
        public CustomBalloonView()
        {
            InitializeComponent();
            TaskbarIcon.AddBalloonClosingHandler(this, OnBalloonClosing);
        }

        private void OnBalloonClosing(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            BalloonToolTipViewModel.IsShown = false;
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            close_Cross.Source = new BitmapImage(new Uri(@"pack://application:,,,/Infrastructure.Common;component/balloontraytip/Images/fileclose_mouseenter.png"));
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            close_Cross.Source = new BitmapImage(new Uri(@"pack://application:,,,/Infrastructure.Common;component/balloontraytip/Images/fileclose.png"));
        }
    }
}
