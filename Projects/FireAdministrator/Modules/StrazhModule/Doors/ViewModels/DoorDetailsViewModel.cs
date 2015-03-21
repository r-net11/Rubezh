﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class DoorDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDDoor Door { get; private set; }

		public DoorDetailsViewModel(SKDDoor door = null)
		{
			if (door == null)
			{
				Title = "Создание точки доступа";
				Door = new SKDDoor()
				{
					Name = "Новая точка доступа",
					No = 1,
					PlanElementUIDs = new List<Guid>(),
				};
				if (SKDManager.Doors.Count != 0)
					Door.No = (ushort)(SKDManager.Doors.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства точки доступа: {0}", door.Name);
				Door = door;
			}

			AvailableDoorTypes = new ObservableCollection<DoorType>();
			AvailableDoorTypes.Add(DoorType.OneWay);
			AvailableDoorTypes.Add(DoorType.TwoWay);

			CopyProperties();
		}

		public void CopyProperties()
		{
			No = Door.No;
			Name = Door.Name;
			Description = Door.Description;
			SelectedDoorType = Door.DoorType;
		}

		int _no;
		public int No
		{
			get { return _no; }
			set
			{
				_no = value;
				OnPropertyChanged("No");
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

		public ObservableCollection<DoorType> AvailableDoorTypes { get; private set; }

		DoorType _selectedDoorType;
		public DoorType SelectedDoorType
		{
			get { return _selectedDoorType; }
			set
			{
				_selectedDoorType = value;
				OnPropertyChanged(() => SelectedDoorType);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			if (No <= 0)
			{
				MessageBoxService.Show("Номер должен быть положительным числом");
				return false;
			}
			if (Door.No != No && SKDManager.Doors.Any(x => x.No == No))
			{
				MessageBoxService.Show("Точка доступа с таким номером уже существует");
				return false;
			}

			Door.No = No;
			Door.Name = Name;
			Door.Description = Description;
			Door.DoorType = SelectedDoorType;
			return true;
		}
	}
}