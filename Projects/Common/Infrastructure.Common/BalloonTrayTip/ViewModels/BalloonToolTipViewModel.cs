using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Drawing;
using System.Windows;

namespace Infrastructure.Common.BalloonTrayTip.ViewModels
{
    public class BalloonToolTipViewModel: WindowBaseViewModel
    {
        public static bool isShown = false;
        public static bool isEmpty = false;

        List<string> titles = new List<string>();
        List<string> texts = new List<string>();
        List<System.Windows.Media.Brush> colors = new List<System.Windows.Media.Brush>();

        public string BalloonTitle
        {
            get { return titles.Last(); }
            set
            {
                titles.Add(value);
                OnPropertyChanged("BalloonTitle");
            }
        }

        public string BalloonText
        {
            get { return texts.Last(); }
            set
            {
                texts.Add(value);
                OnPropertyChanged("BalloonText");
            }
        }

        public System.Windows.Media.Brush BackgroundColor
        {
            get { return colors.Last(); }
            set
            {
                colors.Add(value);
                OnPropertyChanged("BackgroundColor");
            }
        }

        public BalloonToolTipViewModel(string ttl, string txt, System.Windows.Media.Brush clr)
        {
            texts.Clear();
            titles.Clear();
            colors.Clear();
            Title = "";
            BalloonTitle = ttl;
            BalloonText = txt;
            BackgroundColor = clr;
            Test1Command = new RelayCommand(OnTest1);
            isEmpty = false;
        }

        public void AddNote(string ttl, string txt, System.Windows.Media.Brush clr)
        {
            BalloonTitle = ttl;
            BalloonText = txt;
            BackgroundColor = clr;
        }
        public RelayCommand Test1Command { get; private set; }
        void OnTest1()
        {
            if (!chkEmpty())
            {
                titles.Remove(titles.Last());
                OnPropertyChanged("BalloonTitle");
                texts.Remove(texts.Last());
                OnPropertyChanged("BalloonText");
                colors.Remove(colors.Last());
                OnPropertyChanged("BackgroundColor");
            }
        }
        public BalloonToolTipViewModel()
        {
            Title = "";
            Test1Command = new RelayCommand(OnTest1);
            isEmpty = false;
        }

        bool chkEmpty()
        {
            if(titles.Count < 3)
                isEmpty = true;
            return isEmpty;
        }
    }
}
