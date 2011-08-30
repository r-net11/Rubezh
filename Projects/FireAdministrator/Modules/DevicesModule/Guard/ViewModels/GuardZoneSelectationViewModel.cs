using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class GuardZoneSelectationViewModel : SaveCancelDialogContent
    {
        public GuardZoneSelectationViewModel()
        {
            Title = "Выбор зоны";
        }

        public List<Zone> Zones
        {
            get
            {
                return (from Zone zone in FiresecManager.DeviceConfiguration.Zones
                        where zone.ZoneType == ZoneType.Guard
                          select zone).ToList();
            }
        }

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
