using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class XEventViewModel : BaseViewModel
    {
        public XEventViewModel(XEvent xEvent)
        {
            XEvent = xEvent;
        }

        public XEvent XEvent { get; private set; }

        bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
    }
}
