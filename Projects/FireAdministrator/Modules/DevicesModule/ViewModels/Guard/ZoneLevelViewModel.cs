using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;
using System.Windows;
using Infrastructure;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public class ZoneLevelViewModel : BaseViewModel
    {
        public ZoneLevelViewModel()
        {
            ChooseZonesCommand = new RelayCommand(OnChooseZones);
        }
        public ZoneLevelViewModel(ZoneLevel zoneLevel) : this()
        {
            ZoneLevel = zoneLevel;
        }

        public List<ZoneActionType> AvailableActions
        {
            get { return new List<ZoneActionType>(Enum.GetValues(typeof(ZoneActionType)).Cast<ZoneActionType>()); }
        }

        ZoneLevel _zoneLevel;
        public ZoneLevel ZoneLevel
        {
            get { return _zoneLevel; }
            set
            {
                _zoneLevel = value;
                OnPropertyChanged("ZoneLevel");
            }
        }

        public string PresentationZone
        {
            get
            {
                if (string.IsNullOrEmpty(ZoneLevel.ZoneNo) == false)
                {
                    var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == ZoneLevel.ZoneNo);
                    if (zone != null)
                    {
                        return zone.PresentationName;
                    }
                }
                return null;
            }
        }

        public RelayCommand ChooseZonesCommand { get; private set; }
        void OnChooseZones()
        {
            var guardZoneSelectationViewModel = new GuardZoneSelectationViewModel();
            var result = ServiceFactory.UserDialogs.ShowModalWindow(guardZoneSelectationViewModel);
            if (result)
            {
                ZoneLevel.ZoneNo = guardZoneSelectationViewModel.SelectedZone.No;
            }
        }
    }
}
