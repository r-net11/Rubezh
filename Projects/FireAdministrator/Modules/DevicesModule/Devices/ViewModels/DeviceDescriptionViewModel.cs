using Infrastructure.Common;
using FiresecClient;
using System;

namespace DevicesModule.ViewModels
{
    public class DeviceDescriptionViewModel : DialogContent
    {
        public DeviceDescriptionViewModel(Guid deviceUID)
        {
            Title = "Описание устройства";
            Description = FiresecManager.GetDescription(deviceUID);
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
