using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using MuliclientAPI;
using System.Diagnostics;

namespace MultiClientRunner
{
    public class AppItemsViewModels : BaseViewModel
    {
		public static AppItemsViewModels Current;
        public AppItemsViewModels()
        {
			Current = this;
            AppItems = new ObservableCollection<AppItem>();
        }

        public ObservableCollection<AppItem> AppItems { get; private set; }
		public AppItem CurrentAppItem { get; set; }

        AppItem _selectedAppItem;
        public AppItem SelectedAppItem
        {
            get { return _selectedAppItem; }
            set
            {
                if (_selectedAppItem != value)
                {
                    WindowSize windowSize = null;
                    if (_selectedAppItem != null)
                    {
                        windowSize = _selectedAppItem.GetWindowSize();
                    }

                    foreach (var appItem in AppItems)
                    {
                        if (appItem != value)
                        {
                            appItem.Hide();
                        }
                    }
                    if (value != null)
                    {
                        value.Show(windowSize);
                    }
                }

                    _selectedAppItem = value;
					OnPropertyChanged("SelectedAppItem");
            }
        }
    }
}