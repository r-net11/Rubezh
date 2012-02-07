using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;
using System.Collections.ObjectModel;

namespace InstructionsModule.ViewModels
{
    public class InstructionDeviceViewModel : TreeBaseViewModel<InstructionDeviceViewModel>
    {
        public Device Device { get; private set; }

        public InstructionDeviceViewModel(Device device, ObservableCollection<InstructionDeviceViewModel> sourceDevices)
        {
            Children = new ObservableCollection<InstructionDeviceViewModel>();

            Source = sourceDevices;
            Device = device;
        }

        public void Update()
        {
            IsExpanded = false;
            IsExpanded = true;
            OnPropertyChanged("HasChildren");
        }

        public string Address
        {
            get { return Device.PresentationAddress; }
        }

        public Guid UID
        {
            get { return Device.UID; }
        }

        public Driver Driver
        {
            get { return Device.Driver; }
        }
    }
}
