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
        
        public static void Initialize()
        {
        }
        
        public static void ShowWarning(string title, string text = "")
        {
			return;
#if DEBUG
            Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
			{
                if (taskbarIcon == null || taskbarIcon.IsDisposed)
                {
                    taskbarIcon = new TaskbarIcon();
                    taskbarIcon.Visibility = Visibility.Hidden;
                }
                ShowWarning(title, text, Brushes.Black, Brushes.White);
			}));
#endif
        }
        public static void ShowWarning(string title, string text, Brush foregroundColor, Brush backgroundColor)
        {
            try
            {
                balloonToolTipViewModel.AddNote(title, text, foregroundColor, backgroundColor);
                if (BalloonToolTipViewModel.IsShown == false)
                {
                    //ShowTrayWindow(balloonToolTipViewModel);
                    Views.CustomBalloonView customBalloonView = new Views.CustomBalloonView();
                    customBalloonView.DataContext = balloonToolTipViewModel;
                    taskbarIcon.Visibility = Visibility.Hidden;
                    taskbarIcon.ShowCustomBalloon(customBalloonView, System.Windows.Controls.Primitives.PopupAnimation.None, 40000);
                    BalloonToolTipViewModel.IsShown = true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "BalloonHelper.Show");
            }
        }

        static void ShowTrayWindow(WindowBaseViewModel model)
        {
            try
            {
                WindowBaseView win = new WindowBaseView(model);
                win.Visibility = Visibility.Hidden;
                win.AllowsTransparency = true;
                win.Show();
            }
            catch (Exception e)
            {
                Logger.Error(e, "DialogService.ShowTrayWindow");
            }
        }
    }
}