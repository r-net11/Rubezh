using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using GroupControllerModule.Models;

namespace GroupControllerModule.ViewModels
{
    public class DeviceDetailsViewModel : SaveCancelDialogContent
    {
        public DeviceDetailsViewModel(XDevice device)
        {
            Title = "Свойства устройства";
        }

        protected override void Save(ref bool cancel)
        {
        }
    }
}