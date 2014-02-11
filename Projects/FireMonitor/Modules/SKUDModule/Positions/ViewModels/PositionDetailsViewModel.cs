using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class PositionDetailsViewModel : SaveCancelDialogViewModel
	{
		public Position Position { get; private set; }

		public PositionDetailsViewModel(Position position = null)
		{
			if (position == null)
			{
				Title = "Создание должности";
				position = new Position()
				{
					Name = "Новая должность",
				};
			}
			else
			{
				Title = string.Format("Свойства должности: {0}", position.Name);
			}
			Position = position;
			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = Position.Name;
			Description = Position.Description;
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
			return true;
		}

		protected override bool Save()
		{
			Position = new Position()
			{
				Name = Name,
				Description = Description
			};
			return true;
		}
	}
}