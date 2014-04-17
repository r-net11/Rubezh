using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class PositionDetailsViewModel : SaveCancelDialogViewModel
	{
		NewPositionsViewModel PositionsViewModel;
		public Position Position { get; private set; }
		public Organization Organization { get; private set; }

		public PositionDetailsViewModel(NewPositionsViewModel positionsViewModel, Organization orgnaisation, Position position = null)
		{
			PositionsViewModel = positionsViewModel;
			Organization = orgnaisation;
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
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			//if (PositionsViewModel.Positions.Any(x => x.Position.Name == Name && x.Position.UID != Position.UID))
			//{
			//    MessageBoxService.ShowWarning("Название должности совпадает с введеннымы ранее");
			//    return false;
			//}

			Position.Name = Name;
			Position.Description = Description;
			Position.OrganizationUID = Organization.UID;
			return true;
		}
	}
}