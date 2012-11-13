using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.BalloonTrayTip.ViewModels;
using Infrastructure.Common.Windows;

namespace Infrastructure.Common.BalloonTrayTip
{
    public class BalloonHelper
    {
        public static void Show(string title, string text)
        {
            var balloonToolTipViewModel = new BalloonToolTipViewModel(title, text);
            DialogService.ShowTrayWindow(balloonToolTipViewModel);
        }
    }
}
