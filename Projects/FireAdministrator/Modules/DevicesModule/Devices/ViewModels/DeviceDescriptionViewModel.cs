using Infrastructure.Common;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public class DeviceDescriptionViewModel : DialogContent
    {
        public DeviceDescriptionViewModel(string deviceId)
        {
            Title = "Описание устройства";
            Description = FiresecManager.GetDescription(deviceId);
        }

        string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }
    }
}
