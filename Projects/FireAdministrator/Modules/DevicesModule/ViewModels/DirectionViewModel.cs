using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Firesec.CoreConfig;
using FiresecClient;
using System.Collections.ObjectModel;
using FiresecClient.Models;

namespace DevicesModule.ViewModels
{
    public class DirectionViewModel : BaseViewModel
    {
        public DirectionViewModel()
        {
        }

        Direction _direction;

        public void Initialize(Direction direction)
        {
            _direction = direction;

            Zones = new ObservableCollection<ZoneViewModel>();

            foreach (var directionZone in direction.Zones)
            {
                var zone = FiresecManager.CurrentConfiguration.Zones.FirstOrDefault(x => x.No == directionZone.ToString());
                ZoneViewModel zoneViewModel = new ZoneViewModel(zone);
                Zones.Add(zoneViewModel);
            }
        }

        public void Update()
        {
            OnPropertyChanged("Id");
            OnPropertyChanged("Name");
            OnPropertyChanged("Description");
        }

        public int Id
        {
            get { return _direction.Id; }
        }

        public string Name
        {
            get { return _direction.Name; }
        }

        public string Description
        {
            get { return _direction.Description; }
        }

        ObservableCollection<ZoneViewModel> _zones;
        public ObservableCollection<ZoneViewModel> Zones
        {
            get { return _zones; }
            set
            {
                _zones = value;
                OnPropertyChanged("Zones");
            }
        }

        ZoneViewModel _selectedZone;
        public ZoneViewModel SelectedZone
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
