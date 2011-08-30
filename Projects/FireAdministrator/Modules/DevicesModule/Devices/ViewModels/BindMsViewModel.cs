using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class BindMsViewModel : SaveCancelDialogContent
    {
        public BindMsViewModel(string deviceId)
        {
            _deviceId = deviceId;
            Title = "Привязка оборудования";
        }

        string _deviceId;

        protected override void Save()
        {
            ;
        }
    }
}
