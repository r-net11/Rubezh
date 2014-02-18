using System;
using System.Collections.Generic;
using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeCardViewModel : BaseViewModel
	{
		public SKDCard Card { get; private set; }
		UserAccessViewModel UserAccessViewModel;
		List<Guid> ZoneUIDs;

		public EmployeeCardViewModel(UserAccessViewModel userAccessViewModel, SKDCard card)
		{
			RemoveCommand = new RelayCommand(OnRemove);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);

			UserAccessViewModel = userAccessViewModel;
			Card = card;
			ZoneUIDs = new List<Guid>();
			Update();
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

		public string ID
		{
			get { return Card.Series + "/" + Card.Number; }
		}
		public DateTime StartDate
		{
			get { return Card.ValidFrom.GetValueOrDefault(DateTime.MinValue); }
		}
		public DateTime EndDate
		{
			get { return Card.ValidTo.GetValueOrDefault(DateTime.MaxValue); }
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			UserAccessViewModel.RemoveCard(this);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var employeeCardDetailsViewModel = new EmployeeCardDetailsViewModel(Card);
			if (DialogService.ShowModalWindow(employeeCardDetailsViewModel))
			{
				Card = employeeCardDetailsViewModel.Card;
				OnPropertyChanged("ID");
				OnPropertyChanged("StartDate");
				OnPropertyChanged("EndDate");
				Update();
			}
		}
		public bool CanShowProperties()
		{
			return true;
		}

		void Update()
		{
			AllZones = new List<AccessZoneViewModel>();
			RootZone = AddZoneInternal(SKDManager.SKDConfiguration.RootZone, null);
			OnPropertyChanged("RootZones");
			SelectedZone = RootZone;

			foreach (var zone in AllZones)
			{
				if (zone.IsChecked)
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
			var zoneViewModel = new AccessZoneViewModel(zone, Card.CardZones);
			AllZones.Add(zoneViewModel);
			if (parentZoneViewModel != null)
				parentZoneViewModel.AddChild(zoneViewModel);

			foreach (var childZone in zone.Children)
			{
				AddZoneInternal(childZone, zoneViewModel);
			}
			return zoneViewModel;
		}
	}
}