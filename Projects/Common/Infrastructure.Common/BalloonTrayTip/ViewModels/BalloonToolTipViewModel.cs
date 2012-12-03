using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using Common;
using Hardcodet.Wpf.TaskbarNotification;


namespace Infrastructure.Common.BalloonTrayTip.ViewModels
{
    public class BalloonToolTipViewModel: WindowBaseViewModel
    {
        #region StaticFields
        public static bool isShown = false;
        public static bool isEmpty = false;
        #endregion

        #region Fields
        private List<Item> items = new List<Item>();
        #endregion

        #region Properties
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
        #endregion
        
        #region Methods
        public void AddNote(string ttl, string txt, System.Windows.Media.Brush clr)
        {
            items.Add(new Item { title = ttl, text = txt, color = clr });
            OnPropertyChanged("BalloonTitle");
            OnPropertyChanged("BalloonText");
            OnPropertyChanged("BackgroundColor");
        }
        bool chkEmpty()
        {
            if (items.Count < 2)
                isEmpty = true;
            return isEmpty;
        }
        #endregion

        #region Constructors
        public BalloonToolTipViewModel(string ttl, string txt, System.Windows.Media.Brush clr)
        {
            items.Clear();
            Title = "";
            AddNote(ttl, txt, clr);
            OnPropertyChanged("BalloonTitle");
            OnPropertyChanged("BalloonText");
            OnPropertyChanged("BackgroundColor");
            RemoveItemCommand = new RelayCommand(OnRemoveItem);
            ClearCommand = new RelayCommand(OnClear);
            isEmpty = false;
        }
        public BalloonToolTipViewModel()
        {
            Title = "";
            RemoveItemCommand = new RelayCommand(OnRemoveItem);
            ClearCommand = new RelayCommand(OnClear);
            isEmpty = false;
        }
        #endregion

        #region Commands
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
                    this.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Balloon.RemoveItem");
            }

        }

        public RelayCommand ClearCommand { get; private set; }
        void OnClear()
        {
            try
            {
                items.Clear();
                chkEmpty();
                this.Close();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Balloon.ClearItem");
            }

        }
        #endregion

        #region Classes
        class Item
        {
            public string title;
            public string text;
            public System.Windows.Media.Brush color;
        }
        #endregion
    }
}
