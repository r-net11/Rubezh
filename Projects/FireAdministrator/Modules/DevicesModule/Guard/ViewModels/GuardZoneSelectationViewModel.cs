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

            Zones = (from Zone zone in FiresecManager.DeviceConfiguration.Zones
                    where zone.ZoneType == ZoneType.Guard
                    select zone).ToList();

            if (Zones.Count > 0)
                SelectedZone = Zones[0];
        }

        public List<Zone> Zones{get;private set;}

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

        protected override bool CanSave()
        {
            return SelectedZone != null;
        }
    }
}
