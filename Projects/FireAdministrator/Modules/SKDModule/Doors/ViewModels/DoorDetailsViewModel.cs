using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
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
				};
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
			Name = Door.Name;
			Description = Door.Description;
			SelectedDoorType = Door.DoorType;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
					_name = value;
					OnPropertyChanged("Name");
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
					_description = value;
					OnPropertyChanged("Description");
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
			return !string.IsNullOrEmpty(Name) && Name != "Неконтролируемая территория";
		}

		protected override bool Save()
		{
			Door.Name = Name;
			Door.Description = Description;
			Door.DoorType = SelectedDoorType;
			return true;
		}
	}
}