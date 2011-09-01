using Infrastructure.Common;
using System;

namespace DevicesModule.ViewModels
{
    public class AdditionalPropertiesViewModel : SaveCancelDialogContent
    {
        Guid _deviceUID;
        public AdditionalPropertiesViewModel(Guid deviceUID)
        {
            _deviceUID = deviceUID;
            Title = "Дополнительные свойства устройства";
        }

        protected override void Save(ref bool cancel)
        {
            ;
        }
    }
}
