using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DoorDetailsViewModel : SaveCancelDialogViewModel
	{
		public Door Door { get; private set; }

		public DoorDetailsViewModel(Door door = null)
		{
			if (door == null)
			{
				Title = "Создание точки прохода";
				Door = new Door()
				{
					Name = "Новая точка прохода",
				};
			}
			else
			{
				Title = string.Format("Свойства точки прохода: {0}", door.Name);
				Door = door;
			}
			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = Door.Name;
			Description = Door.Description;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged("Name");
				}
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				if (_description != value)
				{
					_description = value;
					OnPropertyChanged("Description");
				}
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
			return true;
		}
	}
}