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
		public XDoor Door;

		public DoorDetailsViewModel(XDoor door = null)
		{
			if (door == null)
			{
				Title = "Создание новой точки доступа";

				Door = new XDoor()
				{
					Name = "Новая точка доступа",
					No = 1
				};
				if (XManager.DeviceConfiguration.Doors.Count != 0)
					Door.No = (ushort)(XManager.DeviceConfiguration.Doors.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства точки доступа: {0}", door.PresentationName);
				Door = door;
			}
			CopyProperties();

			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingDoor in XManager.DeviceConfiguration.Doors)
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
		}

		ushort _no;
		public ushort No
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

		public ObservableCollection<string> AvailableNames { get; private set; }
		public ObservableCollection<string> AvailableDescription { get; private set; }

		protected override bool Save()
		{
			if (Door.No != No && XManager.DeviceConfiguration.Doors.Any(x => x.No == No))
			{
				MessageBoxService.Show("Зона с таким номером уже существует");
				return false;
			}

			Door.No = No;
			Door.Name = Name;
			Door.Description = Description;
			return base.Save();
		}
	}
}