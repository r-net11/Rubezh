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
			RemoveCommand = userAccessViewModel.RemoveCardCommand;
			ChangeZonesCommand = new RelayCommand(OnChangeZones, CanChangeZones);
			ChangeTemplateCommand = new RelayCommand(OnChangeTemplate, CanChangeTemplate);
			ChangeGroupRightsCommand = new RelayCommand(OnChangeGroupRights, CanChangeGroupRights);

			UserAccessViewModel = userAccessViewModel;
			Card = card;
			ID = card.Series+ "/" + card.Number;
			StartDate = card.ValidFrom.GetValueOrDefault(DateTime.MinValue);
			EndDate = card.ValidTo.GetValueOrDefault(DateTime.MaxValue);

			ZoneUIDs = new List<Guid>();
			UpdateZones();
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

		public RelayCommand RemoveCommand { get; private set; }

		public RelayCommand ChangeZonesCommand { get; private set; }
		void OnChangeZones()
		{
			var accessZonesSelectationViewModel = new AccessZonesSelectationViewModel(Card);
			if (DialogService.ShowModalWindow(accessZonesSelectationViewModel))
			{
				UpdateZones();
			}
		}
		public bool CanChangeZones()
		{
			return true;
		}

		public RelayCommand ChangeTemplateCommand { get; private set; }
		void OnChangeTemplate()
		{
			MessageBoxService.ShowWarning("Выбор шаблона");
		}
		public bool CanChangeTemplate()
		{
			return true;
		}

		public RelayCommand ChangeGroupRightsCommand { get; private set; }
		void OnChangeGroupRights()
		{
			MessageBoxService.ShowWarning("Групповое представление прав доступа");
		}
		public bool CanChangeGroupRights()
		{
			return true;
		}

		void UpdateZones()
		{
			AllZones = new List<AccessZoneViewModel>();
			RootZone = AddZoneInternal(SKDManager.SKDConfiguration.RootZone, null);
			SelectedZone = RootZone;

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
	}
}