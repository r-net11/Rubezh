using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows;

namespace CurrentDeviceModule
{
    public class CurrentDeviceModule 
    {
        public CurrentDeviceModule()
        {

        }

        public void Initialize()
        {
            CreateViewModel();
        }

        CurrentDeviceModule currentDeviceViewModel;

        void CreateViewModel()
        {
            currentDeviceViewModel = new CurrentDeviceModule();
        }
    }
}
