using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.BalloonTrayTip.ViewModels
{
    public class BalloonToolTipViewModel: WindowBaseViewModel
    {
        string balloonTitle;
        public string BalloonTitle
        {
            get { return balloonTitle; }
        }

        string balloonText;
        public string BalloonText
        {
            get { return balloonText; }
        }
        
        public BalloonToolTipViewModel(string ttl, string txt)
        {
            balloonTitle = ttl;
            balloonText = txt;
        }
    }
}
