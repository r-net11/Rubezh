using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Common
{
    public class EventAggregator
    {
        public delegate void PropertyChangedDelegate(string Address, string ParentAddress, ComState state, int DeviceState);
        public static event PropertyChangedDelegate PropertyChanged;

        public static void OnPropertyChanged(string Address, string ParentAddress, ComState state, int DeviceState)
        {
            if (PropertyChanged != null)
                PropertyChanged(Address, ParentAddress, state, DeviceState);
        }
    }
}
