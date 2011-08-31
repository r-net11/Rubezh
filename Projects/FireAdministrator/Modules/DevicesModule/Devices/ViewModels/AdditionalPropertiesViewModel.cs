using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class AdditionalPropertiesViewModel : SaveCancelDialogContent
    {
        public AdditionalPropertiesViewModel(string deviceId)
        {
            _deviceId = deviceId;
            Title = "Дополнительные свойства устройства";
        }

        string _deviceId;

        protected override void Save(ref bool cancel)
        {
            ;
        }
    }
}
