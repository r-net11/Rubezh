using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Drawing;
using System.Windows;
using Common;


namespace Infrastructure.Common.BalloonTrayTip.ViewModels
{
    public class BalloonToolTipViewModel: WindowBaseViewModel
    {
        public static bool isShown = false;
        public static bool isEmpty = false;

        
        private List<Item> items = new List<Item>();
        
        public string BalloonTitle
        {
            get { return items.Last().title; }
        }

        public string BalloonText
        {
            get { return items.Last().text; }
        }

        public System.Windows.Media.Brush BackgroundColor
        {
            get { return items.Last().color; }
        }

        public BalloonToolTipViewModel(string ttl, string txt, System.Windows.Media.Brush clr)
        {
            items.Clear();
            Title = "";
            AddNote(ttl, txt, clr);
            OnPropertyChanged("BalloonTitle");
            OnPropertyChanged("BalloonText");
            OnPropertyChanged("BackgroundColor");
            RemoveItemCommand = new RelayCommand(OnRemoveItem);
            isEmpty = false;
        }

        public void AddNote(string ttl, string txt, System.Windows.Media.Brush clr)
        {
            items.Add(new Item { title = ttl, text = txt, color = clr });
            OnPropertyChanged("BalloonTitle");
            OnPropertyChanged("BalloonText");
            OnPropertyChanged("BackgroundColor");
        }
        public RelayCommand RemoveItemCommand { get; private set; }
        void OnRemoveItem()
        {
            try
            {
                if (!chkEmpty())
                {
                    items.Remove(items.Last());
                    OnPropertyChanged("BalloonTitle");
                    OnPropertyChanged("BalloonText");
                    OnPropertyChanged("BackgroundColor");
                }
                else
                {
                    items.Clear();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Balloon.RemoveItem");
            }
            
        }

        public BalloonToolTipViewModel()
        {
            Title = "";
            RemoveItemCommand = new RelayCommand(OnRemoveItem);
            isEmpty = false;
        }

        bool chkEmpty()
        {
            if(items.Count < 2)
                isEmpty = true;
            return isEmpty;
        }

        class Item
        {
            public string title;
            public string text;
            public System.Windows.Media.Brush color;
        }
    }
}
