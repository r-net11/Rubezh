using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public class GuardZoneSelectationViewModel : DialogContent
    {
        public GuardZoneSelectationViewModel()
        {
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
            Title = "Выбор зоны";
        }

        public List<Zone> Zones
        {
            get
            {
                return (from Zone zone in FiresecManager.DeviceConfiguration.Zones
                        where zone.ZoneType == "1"
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

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
