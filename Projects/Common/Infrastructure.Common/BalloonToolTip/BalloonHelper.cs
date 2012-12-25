using System;
using System.Windows.Media;
using System.Windows.Threading;
using Common;
using Infrastructure.Common.BalloonTrayTip.ViewModels;

namespace Infrastructure.Common.BalloonTrayTip
{
    public class BalloonHelper
    {
        static BalloonToolTipViewModel balloonToolTipViewModel = new BalloonToolTipViewModel();
        static Views.BalloonToolTipView customBalloonView;

        public static void Show(string title, string text = "")
        {
            Show(title, text, Brushes.Black, Brushes.White);
        }

        public static void Show(string title, string text, Brush foregroundColor, Brush backgroundColor)
        {
            Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
            {
                try
                {
                    if (customBalloonView == null)
                    {
                        customBalloonView = new Views.BalloonToolTipView();
                        customBalloonView.DataContext = balloonToolTipViewModel;
                    }
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