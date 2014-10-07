using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace GKModule.ViewModels
{
	public class GuardZoneDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKGuardZone Zone;

		public GuardZoneDetailsViewModel(GKGuardZone zone = null)
		{
			if (zone == null)
			{
				Title = "Создание новой зоны";

				Zone = new GKGuardZone()
				{
					Name = "Новая зона",
					No = 1
				};
				if (GKManager.DeviceConfiguration.GuardZones.Count != 0)
					Zone.No = (ushort)(GKManager.DeviceConfiguration.GuardZones.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства зоны: {0}", zone.PresentationName);
				Zone = zone;
			}

			AvailableGuardZoneEnterMethods = new ObservableCollection<GKGuardZoneEnterMethod>(Enum.GetValues(typeof(GKGuardZoneEnterMethod)).Cast<GKGuardZoneEnterMethod>());

			CopyProperties();

			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingZone in GKManager.DeviceConfiguration.GuardZones)
			{
				availableNames.Add(existingZone.Name);
				availableDescription.Add(existingZone.Description);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);
			AvailableDescription = new ObservableCollection<string>(availableDescription);
		}

		void CopyProperties()
		{
			No = Zone.No;
			Name = Zone.Name;
			Description = Zone.Description;
			SelectedGuardZoneEnterMethod = Zone.GuardZoneEnterMethod;
			SetGuardLevel = Zone.SetGuardLevel;
			ResetGuardLevel = Zone.ResetGuardLevel;
			SetAlarmLevel = Zone.SetAlarmLevel;
			Delay = Zone.Delay;
			ResetDelay = Zone.ResetDelay;
			AlarmDelay = Zone.AlarmDelay;
		}

		int _no;
		public int No
		{
			get { return _no; }
			set
			{
				_no = value;
				OnPropertyChanged(() => No);
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		public ObservableCollection<GKGuardZoneEnterMethod> AvailableGuardZoneEnterMethods { get; private set; }

		GKGuardZoneEnterMethod _selectedGuardZoneEnterMethod;
		public GKGuardZoneEnterMethod SelectedGuardZoneEnterMethod
		{
			get { return _selectedGuardZoneEnterMethod; }
			set
			{
				_selectedGuardZoneEnterMethod = value;
				OnPropertyChanged(() => SelectedGuardZoneEnterMethod);
			}
		}

		int _setGuardLevel;
		public int SetGuardLevel
		{
			get { return _setGuardLevel; }
			set
			{
				_setGuardLevel = value;
				OnPropertyChanged(() => SetGuardLevel);
			}
		}

		int _resetGuardLevel;
		public int ResetGuardLevel
		{
			get { return _resetGuardLevel; }
			set
			{
				_resetGuardLevel = value;
				OnPropertyChanged(() => ResetGuardLevel);
			}
		}

		int _setAlarmLevel;
		public int SetAlarmLevel
		{
			get { return _setAlarmLevel; }
			set
			{
				_setAlarmLevel = value;
				OnPropertyChanged(() => SetAlarmLevel);
			}
		}

		int _delay;
		public int Delay
		{
			get { return _delay; }
			set
			{
				_delay = value;
				OnPropertyChanged(() => Delay);
			}
		}

		int _resetDelay;
		public int ResetDelay
		{
			get { return _resetDelay; }
			set
			{
				_resetDelay = value;
				OnPropertyChanged(() => ResetDelay);
			}
		}

		int _alarmDelay;
		public int AlarmDelay
		{
			get { return _alarmDelay; }
			set
			{
				_alarmDelay = value;
				OnPropertyChanged(() => AlarmDelay);
			}
		}

		public ObservableCollection<string> AvailableNames { get; private set; }
		public ObservableCollection<string> AvailableDescription { get; private set; }

		protected override bool Save()
		{
			if (No <= 0)
			{
				MessageBoxService.ShowExtended("Номер должен быть положительным числом");
				return false;
			}
			if (Zone.No != No && GKManager.DeviceConfiguration.GuardZones.Any(x => x.No == No))
			{
				MessageBoxService.ShowExtended("Зона с таким номером уже существует");
				return false;
			}

			Zone.No = No;
			Zone.Name = Name;
			Zone.Description = Description;
			Zone.GuardZoneEnterMethod = SelectedGuardZoneEnterMethod;
			Zone.SetGuardLevel = SetGuardLevel;
			Zone.ResetGuardLevel = ResetGuardLevel;
			Zone.SetAlarmLevel = SetAlarmLevel;
			Zone.Delay = Delay;
			Zone.ResetDelay = ResetDelay;
			Zone.AlarmDelay = AlarmDelay;
			return base.Save();
		}
	}
}