using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ZoneLevelViewModel : BaseViewModel
    {
        public ZoneLevel ZoneLevel { get; private set; }

        public ZoneLevelViewModel(ZoneLevel zoneLevel)
        {
            ChooseZonesCommand = new RelayCommand(OnChooseZones);
            ZoneLevel = zoneLevel;
        }

        public List<ZoneActionType> AvailableActions
        {
            get { return new List<ZoneActionType>(Enum.GetValues(typeof(ZoneActionType)).Cast<ZoneActionType>()); }
        }

        public string PresentationZone
        {
            get
            {
                if (ZoneLevel.ZoneNo.HasValue)
                {
                    var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == ZoneLevel.ZoneNo);
                    if (zone != null)
                        return zone.PresentationName;
                }
                return null;
            }
        }

        public RelayCommand ChooseZonesCommand { get; private set; }
        void OnChooseZones()
        {
            var guardZoneSelectationViewModel = new GuardZoneSelectationViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(guardZoneSelectationViewModel))
            {
                ZoneLevel.ZoneNo = guardZoneSelectationViewModel.SelectedZone.No;
                OnPropertyChanged("PresentationZone");
            }
        }
    }
}