using Infrastructure.Common;
using FiresecClient;
using System;

namespace DevicesModule.ViewModels
{
    public class DeviceDescriptionViewModel : DialogContent
    {
        public DeviceDescriptionViewModel(Guid deviceUID, string description)
        {
            Title = "Описание устройства";
            Description = description;
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
