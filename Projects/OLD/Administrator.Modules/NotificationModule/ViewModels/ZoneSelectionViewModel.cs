using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.ViewModels;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace NotificationModule.ViewModels
{
	public class ZoneSelectionViewModel : SaveCancelDialogViewModel
	{
		public ZoneSelectionViewModel(List<Guid> zonesList)
		{
			AddOneCommand = new RelayCommand(OnAddOne, CanAddOne);
			RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemoveOne);
			AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);

			Title = "Выбор зоны";

			ChosenZonesList = new List<Guid>(zonesList);
			ChosenZones = new ObservableCollection<ZoneViewModel>();
			AvailableZones = new ObservableCollection<ZoneViewModel>();

			InitializeZones();

			if (ChosenZones.IsNotNullOrEmpty())
				SelectedChosenZone = ChosenZones[0];
		}

		private void InitializeZones()
		{
			foreach (var zone in FiresecManager.Zones)
			{
				var zoneViewModel = new ZoneViewModel(zone);
				if (ChosenZonesList.IsNotNullOrEmpty())
				{
					var chosenZone = ChosenZonesList.FirstOrDefault(x => x == zoneViewModel.Zone.UID);
					if (chosenZone != Guid.Empty)
						ChosenZones.Add(zoneViewModel);
					else
						AvailableZones.Add(zoneViewModel);
				}
				else
				{
					AvailableZones.Add(zoneViewModel);
				}
			}

			if (ChosenZones.IsNotNullOrEmpty())
				SelectedChosenZone = ChosenZones[0];
			if (AvailableZones.IsNotNullOrEmpty())
				SelectedAvailableZone = AvailableZones[0];
		}

		public List<Guid> ChosenZonesList { get; set; }

		public ObservableCollection<ZoneViewModel> AvailableZones { get; set; }

		public ObservableCollection<ZoneViewModel> ChosenZones { get; set; }

		ZoneViewModel _selectedAvailableZone;

		public ZoneViewModel SelectedAvailableZone
		{
			get { return _selectedAvailableZone; }
			set
			{
				_selectedAvailableZone = value;
				OnPropertyChanged(() => SelectedAvailableZone);
			}
		}

		ZoneViewModel _selectedChosenZone;

		public ZoneViewModel SelectedChosenZone
		{
			get { return _selectedChosenZone; }
			set
			{
				_selectedChosenZone = value;
				OnPropertyChanged(() => SelectedChosenZone);
			}
		}

		public bool CanAddOne()
		{
			return SelectedAvailableZone != null;
		}

		public bool CanAddAll()
		{
			return AvailableZones.IsNotNullOrEmpty();
		}

		public bool CanRemoveOne()
		{
			return SelectedChosenZone != null;
		}

		public bool CanRemoveAll()
		{
			return ChosenZones.IsNotNullOrEmpty();
		}

		public RelayCommand AddOneCommand { get; private set; }

		private void OnAddOne()
		{
			ChosenZones.Add(SelectedAvailableZone);
			AvailableZones.Remove(SelectedAvailableZone);
			if (AvailableZones.IsNotNullOrEmpty())
				SelectedAvailableZone = AvailableZones[0];
			if (ChosenZones.IsNotNullOrEmpty())
				SelectedChosenZone = ChosenZones[0];
		}

		public RelayCommand AddAllCommand { get; private set; }

		private void OnAddAll()
		{
			foreach (var availableZone in AvailableZones)
			{
				ChosenZones.Add(availableZone);
			}

			AvailableZones.Clear();
			SelectedAvailableZone = null;
			if (ChosenZones.IsNotNullOrEmpty())
				SelectedChosenZone = ChosenZones[0];
		}

		public RelayCommand RemoveAllCommand { get; private set; }

		private void OnRemoveAll()
		{
			foreach (var instructionZone in ChosenZones)
			{
				AvailableZones.Add(instructionZone);
			}

			ChosenZones.Clear();
			SelectedChosenZone = null;
			if (AvailableZones.IsNotNullOrEmpty())
				SelectedAvailableZone = AvailableZones[0];
		}

		public RelayCommand RemoveOneCommand { get; private set; }

		private void OnRemoveOne()
		{
			AvailableZones.Add(SelectedChosenZone);
			ChosenZones.Remove(SelectedChosenZone);
			if (AvailableZones.IsNotNullOrEmpty())
				SelectedAvailableZone = AvailableZones[0];
			if (ChosenZones.IsNotNullOrEmpty())
				SelectedChosenZone = ChosenZones[0];
		}

		protected override bool Save()
		{
			ChosenZonesList = new List<Guid>(from zone in ChosenZones select zone.Zone.UID);
			return base.Save();
		}
	}
}