using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class IndicatorZoneSelectionViewModel : SaveCancelDialogViewModel
    {
		public List<Guid> Zones { get; private set; }

		public IndicatorZoneSelectionViewModel(List<Guid> zones, Device device)
		{
			Title = "Свойства индикатора";

			AddOneCommand = new RelayCommand(OnAddOne, CanAdd);
			RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAdd);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemove);

			Zones = zones;
			TargetZones = new ObservableCollection<ZoneViewModel>();
			SourceZones = new ObservableCollection<ZoneViewModel>();

			foreach (var zone in FiresecManager.FiresecConfiguration.GetChannelZones(device))
			{
				var zoneViewModel = new ZoneViewModel(zone);

                if (Zones.Contains(zone.UID))
					TargetZones.Add(zoneViewModel);
				else
					SourceZones.Add(zoneViewModel);
			}

			SelectedTargetZone = TargetZones.FirstOrDefault();
			SelectedSourceZone = SourceZones.FirstOrDefault();
		}

        public ObservableCollection<ZoneViewModel> SourceZones { get; set; }

        ZoneViewModel _selectedSourceZone;
        public ZoneViewModel SelectedSourceZone
        {
            get { return _selectedSourceZone; }
            set
            {
                _selectedSourceZone = value;
                OnPropertyChanged("SelectedSourceZone");
            }
        }

        public ObservableCollection<ZoneViewModel> TargetZones { get; set; }

        ZoneViewModel _selectedTargetZone;
        public ZoneViewModel SelectedTargetZone
        {
            get { return _selectedTargetZone; }
            set
            {
                _selectedTargetZone = value;
                OnPropertyChanged("SelectedTargetZone");
            }
        }

        public RelayCommand AddOneCommand { get; private set; }
        void OnAddOne()
        {
            TargetZones.Add(SelectedSourceZone);
            SelectedTargetZone = SelectedSourceZone;
            SourceZones.Remove(SelectedSourceZone);
			SelectedSourceZone = SourceZones.FirstOrDefault();
        }

        public RelayCommand RemoveOneCommand { get; private set; }
        void OnRemoveOne()
        {
            SourceZones.Add(SelectedTargetZone);
            SelectedSourceZone = SelectedTargetZone;
            TargetZones.Remove(SelectedTargetZone);
			SelectedTargetZone = TargetZones.FirstOrDefault();
        }

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            foreach (var zoneViewModel in SourceZones)
            {
                TargetZones.Add(zoneViewModel);
            }
            SourceZones.Clear();
			SelectedTargetZone = TargetZones.FirstOrDefault();
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            foreach (var zoneViewModel in TargetZones)
            {
                SourceZones.Add(zoneViewModel);
            }
            TargetZones.Clear();
			SelectedSourceZone = SourceZones.FirstOrDefault();
        }

        bool CanAdd()
        {
            return SelectedSourceZone != null;
        }

        bool CanRemove()
        {
            return SelectedTargetZone != null;
        }

		protected override bool Save()
		{
			Zones = new List<Guid>();
			foreach (var zone in TargetZones)
			{
                Zones.Add(zone.Zone.UID);
			}
			return base.Save();
		}
    }
}