using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.BalloonTrayTip.ViewModels
{
    public class BalloonToolTipViewModel: WindowBaseViewModel
    {

        public static string BalloonTitle { get; private set; }
        public static string BalloonText{get; private set;}
        public static bool IsCustom { get; set; }
        

        public BalloonToolTipViewModel(string ttl, string txt, bool cstm = false)
        {
            Title = "";
            BalloonTitle = ttl;
            BalloonText = txt;
            IsCustom = cstm;
        }

        public BalloonToolTipViewModel()
        {
            ;
        }
    }
}
