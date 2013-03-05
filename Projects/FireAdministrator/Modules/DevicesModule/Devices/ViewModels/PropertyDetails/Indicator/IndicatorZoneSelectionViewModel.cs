using System;
using System.Collections;
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

			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
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

		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedSourceZones;
		void OnAdd(object parameter)
		{
			SelectedSourceZones = (IList)parameter;
			var zoneViewModels = new List<ZoneViewModel>();
			foreach (var selectedZone in SelectedSourceZones)
			{
				ZoneViewModel zoneViewModel = selectedZone as ZoneViewModel;
				if (zoneViewModel != null)
					zoneViewModels.Add(zoneViewModel);
			}
			foreach (var zoneViewModel in zoneViewModels)
			{
				TargetZones.Add(zoneViewModel);
				SourceZones.Remove(zoneViewModel);
			}

			OnPropertyChanged("SourceZones");
			SelectedSourceZone = SourceZones.FirstOrDefault();
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedTargetZones;
		void OnRemove(object parameter)
		{
			SelectedTargetZones = (IList)parameter;
			var zoneViewModels = new List<ZoneViewModel>();
			foreach (var selectedZone in SelectedTargetZones)
			{
				ZoneViewModel zoneViewModel = selectedZone as ZoneViewModel;
				if (zoneViewModel != null)
					zoneViewModels.Add(zoneViewModel);
			}
			foreach (var zoneViewModel in zoneViewModels)
			{
				SourceZones.Add(zoneViewModel);
				TargetZones.Remove(zoneViewModel);
			}

			OnPropertyChanged("TargetZones");
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

		bool CanAdd(object parameter)
		{
			return SelectedSourceZone != null;
		}

		bool CanRemove(object parameter)
		{
			return SelectedTargetZone != null;
		}

		protected override bool Save()
		{
			Zones = new List<Guid>();
			var sortedZones = (from ZoneViewModel zoneViewModel in TargetZones orderby zoneViewModel.Zone.No select zoneViewModel);
			foreach (var zone in sortedZones)
			{
				Zones.Add(zone.Zone.UID);
			}
			return base.Save();
		}
	}
}