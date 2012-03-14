using GroupControllerModule.Models;
using Infrastructure.Common;

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