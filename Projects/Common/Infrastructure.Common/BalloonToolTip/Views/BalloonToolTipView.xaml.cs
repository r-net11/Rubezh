using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Infrastructure.Common.BalloonTrayTip.ViewModels;
using System;
using System.Windows.Media.Animation;

namespace Infrastructure.Common.BalloonTrayTip.Views
{
    public partial class BalloonToolTipView : UserControl
    {
        System.Windows.Threading.DispatcherTimer dispatcherTimer;

        public BalloonToolTipView()
        {
            InitializeComponent();
            this.Visibility = Visibility.Hidden;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromSeconds(40);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Storyboard sb = this.FindResource("ClosingAnimation") as Storyboard;
            sb.Completed += new EventHandler(Closing_Completed);
            BeginStoryboard(sb);
        }

        private void image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            HideBalloon();
        }

        private void Title_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            if (this.Visibility == Visibility.Hidden)
            {
                ShowBalloon();
            }
            dispatcherTimer.Stop();
            dispatcherTimer.Start();
        }

        private void grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (BalloonToolTipViewModel.IsEmpty)
                HideBalloon();
        }

        private void ShowBalloon()
        {
            this.Visibility = Visibility.Visible;
            Storyboard sb = this.FindResource("OpeningAnimation") as Storyboard;
            BeginStoryboard(sb);
        }

        private void HideBalloon()
        {
            Storyboard sb = this.FindResource("ClosingAnimation") as Storyboard;
            sb.Completed += new EventHandler(Closing_Completed);
            BeginStoryboard(sb);
        }

        private void Closing_Completed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
