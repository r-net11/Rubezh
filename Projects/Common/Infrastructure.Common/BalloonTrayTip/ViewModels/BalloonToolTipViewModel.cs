using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Drawing;

namespace Infrastructure.Common.BalloonTrayTip.ViewModels
{
    public class BalloonToolTipViewModel: WindowBaseViewModel
    {

        public string BalloonTitle { get; private set; }
        public string BalloonText{get; private set;}
        private System.Windows.Media.Brush backgroundColor = System.Windows.Media.Brushes.DarkSeaGreen;

        public System.Windows.Media.Brush BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                OnPropertyChanged("BackgroundColor");
            }
        }

        public BalloonToolTipViewModel(string ttl, string txt, System.Windows.Media.Brush clr)
        {
            Title = "";
            BalloonTitle = ttl;
            BalloonText = txt;
            BackgroundColor = clr;
        }

        public BalloonToolTipViewModel()
        {
            ;
        }
    }
}
