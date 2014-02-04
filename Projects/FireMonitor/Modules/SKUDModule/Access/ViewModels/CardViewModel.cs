using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class CardViewModel : BaseViewModel
	{
		public SKDCard Card { get; private set; }

		public CardViewModel(SKDCard card)
		{
			ChangeZonesCommand = new RelayCommand(OnChangeZones, CanChangeZones);
			ChangeTemplateCommand = new RelayCommand(OnChangeTemplate, CanChangeTemplate);
			DeleteZoneCommand = new RelayCommand(OnDeleteZone, CanDeleteZone);
			ChangeGroupRightsCommand = new RelayCommand(OnChangeGroupRights, CanChangeGroupRights);

			Card = card;
			ID = card.IDFamily + "/" + card.IDNo;
			StartDate = card.StartDate;
			EndDate = card.EndDate;

			UpdateCardAccessItems();
		}

		bool _isBlocked;
		public bool IsBlocked
		{
			get { return _isBlocked; }
			set
			{
				_isBlocked = value;
				OnPropertyChanged("IsBlocked");
			}
		}

		public string ID { get; private set; }
		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }

		public RelayCommand ChangeZonesCommand { get; private set; }
		void OnChangeZones()
		{
			var accessZonesSelectationViewModel = new AccessZonesSelectationViewModel();
			if (DialogService.ShowModalWindow(accessZonesSelectationViewModel))
			{
				UpdateCardAccessItems();
			}
		}
		public bool CanChangeZones()
		{
			return true;
		}

		public RelayCommand ChangeTemplateCommand { get; private set; }
		void OnChangeTemplate()
		{
		}
		public bool CanChangeTemplate()
		{
			return true;
		}

		public RelayCommand DeleteZoneCommand { get; private set; }
		void OnDeleteZone()
		{
		}
		public bool CanDeleteZone()
		{
			return true;
		}

		public RelayCommand ChangeGroupRightsCommand { get; private set; }
		void OnChangeGroupRights()
		{
		}
		public bool CanChangeGroupRights()
		{
			return true;
		}

		void UpdateCardAccessItems()
		{
			AllZones = new List<AccessZoneViewModel>();
			RootZone = AddZoneInternal(SKDManager.SKDConfiguration.RootZone, null);

			foreach (var zone in AllZones)
			{
				zone.ExpandToThis();
			}
		}

		public List<AccessZoneViewModel> AllZones;

		AccessZoneViewModel _selectedZone;
		public AccessZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("SelectedZone");
				InitializeDevices();
			}
		}

		AccessZoneViewModel _rootZone;
		public AccessZoneViewModel RootZone
		{
			get { return _rootZone; }
			private set
			{
				_rootZone = value;
				OnPropertyChanged("RootZone");
			}
		}

		public AccessZoneViewModel[] RootZones
		{
			get { return new AccessZoneViewModel[] { RootZone }; }
		}

		AccessZoneViewModel AddZoneInternal(SKDZone zone, AccessZoneViewModel parentZoneViewModel)
		{
			var zoneViewModel = new AccessZoneViewModel(zone);
			AllZones.Add(zoneViewModel);
			if (parentZoneViewModel != null)
				parentZoneViewModel.AddChild(zoneViewModel);

			foreach (var childZone in zone.Children)
			{
				AddZoneInternal(childZone, zoneViewModel);
			}
			return zoneViewModel;
		}


		void InitializeDevices()
		{
			Devices = new ObservableCollection<CardDeviceViewModel>();
			if (SelectedZone != null)
			{
				foreach (var device in SKDManager.Devices)
				{
					if (device.OuterZoneUID == SelectedZone.Zone.UID)
					{
						var cardDeviceViewModel = new CardDeviceViewModel(device);
						Devices.Add(cardDeviceViewModel);
					}
				}
			}
		}

		ObservableCollection<CardDeviceViewModel> _devices;
		public ObservableCollection<CardDeviceViewModel> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged("Devices");
			}
		}
	}
}