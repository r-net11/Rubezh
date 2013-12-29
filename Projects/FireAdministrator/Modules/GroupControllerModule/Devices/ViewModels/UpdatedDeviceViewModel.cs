using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class UpdatedDeviceViewModel:BaseViewModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public XDevice Device;
        public UpdatedDeviceViewModel(XDevice device)
        {
            Device = device;
            Name = device.ShortName;
            Address = device.DottedPresentationAddress;
        }

        bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
    }
}
