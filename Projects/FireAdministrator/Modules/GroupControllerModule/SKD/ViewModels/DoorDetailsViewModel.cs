using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DoorDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKDoor Door;

		public DoorDetailsViewModel(GKDoor door = null)
		{
			if (door == null)
			{
				Title = "Создание новой точки доступа";

				Door = new GKDoor()
				{
					Name = "Новая точка доступа",
					No = 1,
					PlanElementUIDs = new List<Guid>(),
				};
				if (GKManager.DeviceConfiguration.Doors.Count != 0)
					Door.No = (ushort)(GKManager.DeviceConfiguration.Doors.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства точки доступа: {0}", door.PresentationName);
				Door = door;
			}

			AvailableDoorTypes = new ObservableCollection<GKDoorType>();
			AvailableDoorTypes.Add(GKDoorType.OneWay);
			AvailableDoorTypes.Add(GKDoorType.TwoWay);
			SelectedDoorType = Door.DoorType;

			CopyProperties();

			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingDoor in GKManager.DeviceConfiguration.Doors)
			{
				availableNames.Add(existingDoor.Name);
				availableDescription.Add(existingDoor.Description);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);
			AvailableDescription = new ObservableCollection<string>(availableDescription);
		}

		void CopyProperties()
		{
			No = Door.No;
			Name = Door.Name;
			Description = Door.Description;
			Delay = Door.Delay;
			EnterLevel = Door.EnterLevel;
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

		public ObservableCollection<GKDoorType> AvailableDoorTypes { get; private set; }

		GKDoorType _selectedDoorType;
		public GKDoorType SelectedDoorType
		{
			get { return _selectedDoorType; }
			set
			{
				_selectedDoorType = value;
				OnPropertyChanged(() => SelectedDoorType);
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

		int _enterLevel;
		public int EnterLevel
		{
			get { return _enterLevel; }
			set
			{
				_enterLevel = value;
				OnPropertyChanged(() => EnterLevel);
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
			if (Door.No != No && GKManager.DeviceConfiguration.Doors.Any(x => x.No == No))
			{
				MessageBoxService.ShowExtended("Зона с таким номером уже существует");
				return false;
			}

			Door.No = No;
			Door.Name = Name;
			Door.Description = Description;
			Door.Delay = Delay;
			Door.EnterLevel = EnterLevel;
			return base.Save();
		}
	}
}