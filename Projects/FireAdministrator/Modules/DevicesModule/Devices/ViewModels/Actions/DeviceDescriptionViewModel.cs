using System;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
    public class DeviceDescriptionViewModel : DialogViewModel
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