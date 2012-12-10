using System;
using System.Windows.Media;
using Infrastructure.Common.BalloonTrayTip.ViewModels;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows.Views;
using Common;
using System.Windows;
using System.Windows.Threading;
using Infrastructure.Common.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace Infrastructure.Common.BalloonTrayTip
{
    public class BalloonHelper
    {
        static BalloonToolTipViewModel balloonToolTipViewModel = new BalloonToolTipViewModel();
        static TaskbarIcon taskbarIcon;
        static Views.BalloonToolTipView customBalloonView;
        
        public static void Initialize()
        {
            try
            {
                if (taskbarIcon == null)
                {
                    taskbarIcon = new TaskbarIcon();
                    taskbarIcon.Visibility = Visibility.Hidden;
                    customBalloonView = new Views.BalloonToolTipView();
                    customBalloonView.DataContext = balloonToolTipViewModel;
                    taskbarIcon.ShowCustomBalloon(customBalloonView, System.Windows.Controls.Primitives.PopupAnimation.None, null);
                    customBalloonView.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "BalloonHelper.Initialize");
            }
            
        }
        
        public static void Show(string title, string text = "")
        {
#if DEBUG
            Show(title, text, Brushes.Black, Brushes.White);
#endif
        }

        public static void Show(string title, string text, Brush foregroundColor, Brush backgroundColor)
        {
            Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
            {
                try
                {
                    balloonToolTipViewModel.Add(title, text, foregroundColor, backgroundColor);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "BalloonHelper.Show");
                }
            }));
        }
    }
}