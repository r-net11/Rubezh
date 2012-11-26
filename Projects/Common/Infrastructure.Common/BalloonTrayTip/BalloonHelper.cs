using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.BalloonTrayTip.ViewModels;
using Infrastructure.Common.Windows;
using System.Drawing;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows.Views;
using Common;
using System.Windows;

namespace Infrastructure.Common.BalloonTrayTip
{
    public class BalloonHelper
    {
        static BalloonToolTipViewModel balloonToolTipViewModel = new BalloonToolTipViewModel("", "", System.Windows.Media.Brushes.OldLace);
        

        public static void Show(string title, string text, System.Windows.Media.Brush clr)
        {
            balloonToolTipViewModel.AddNote(title, text, clr);
            if (BalloonToolTipViewModel.isShown == false)
            {
                ShowTrayWindow(balloonToolTipViewModel);
                BalloonToolTipViewModel.isShown = true;
            }
                
        }
        
        static bool ShowTrayWindow(WindowBaseViewModel model, bool allowsTransparency = true)
        {
            //WindowBaseView win = new WindowBaseView(model);
            //win.Visibility = Visibility.Hidden;
            //win.Show();
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

        public static void ShowConflagration(string title, string text)
        {
            Show(title, text, System.Windows.Media.Brushes.Tomato);
        }

        public static void ShowWarning(string title, string text)
        {
            Show(title, text, System.Windows.Media.Brushes.Goldenrod);
        }

        public static void ShowNotification(string title, string text)
        {
            Show(title, text, System.Windows.Media.Brushes.CornflowerBlue);
        }
    }
}