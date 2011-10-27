using Infrastructure.Common;
using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using System.Linq;

namespace PlansModule.ViewModels
{
    public class ZonePropertiesViewModel : SaveCancelDialogContent
    {
        public ZonePropertiesViewModel(ulong? zoneNo)
        {
            Title = "Свойства фигуры: Зона";
            Zones = new List<Zone>(FiresecManager.DeviceConfiguration.Zones);
            if (zoneNo.HasValue)
                SelectedZone = Zones.FirstOrDefault(x => x.No == zoneNo.Value);
        }

        public List<Zone> Zones { get; private set; }

        Zone _selectedZone;
        public Zone SelectedZone
        {
            get { return _selectedZone; }
            set
            {
                _selectedZone = value;
                OnPropertyChanged("SelectedZone");
            }
        }
    }
}
