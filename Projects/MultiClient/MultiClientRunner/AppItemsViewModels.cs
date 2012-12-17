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
    }
}