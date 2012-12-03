using System;
using System.Windows.Media;
using Infrastructure.Common.BalloonTrayTip.ViewModels;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows.Views;
using Common;
using System.Windows;
using System.Windows.Threading;

namespace Infrastructure.Common.BalloonTrayTip
{
    public class BalloonHelper
    {
        static BalloonToolTipViewModel balloonToolTipViewModel = new BalloonToolTipViewModel();
        
        public static void ShowWarning(string title, string text = "")
        {
#if DEBUG
			Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
			{
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
                    ShowTrayWindow(balloonToolTipViewModel);
                    BalloonToolTipViewModel.IsShown = true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "BalloonHelper.Show");
            }
        }

        static bool ShowTrayWindow(WindowBaseViewModel model, bool allowsTransparency = true)
        {
            try
            {
                WindowBaseView win = new WindowBaseView(model);
                win.Visibility = Visibility.Hidden;
                win.AllowsTransparency = allowsTransparency;
                bool? result = win.ShowDialog();
                return result.HasValue ? result.Value : false;
            }
            catch (Exception e)
            {
                Logger.Error(e, "DialogService.ShowTrayWindow");
            }
            return false;
        }
    }
}