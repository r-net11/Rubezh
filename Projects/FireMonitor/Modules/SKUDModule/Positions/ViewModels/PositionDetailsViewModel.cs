using System;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class PositionDetailsViewModel : SaveCancelDialogViewModel
	{
		PositionsViewModel PositionsViewModel;
		public Position Position { get; private set; }
		public ShortPosition ShortPosition
		{
			get
			{
				return new ShortPosition
				{
					UID = Position.UID,
					Name = Position.Name,
					Description = Position.Description,
					OrganisationUID = Position.OrganizationUID
				};
			}
		}

		public Organisation Organization { get; private set; }

		public PositionDetailsViewModel(PositionsViewModel positionsViewModel, Organisation orgnaisation, Guid? positionUID = null)
		{
			PositionsViewModel = positionsViewModel;
			Organization = orgnaisation;
			if (positionUID == null)
			{
				Title = "Создание должности";
				Position = new Position()
				{
					Name = "Новая должность",
					OrganizationUID = Organization.UID
				};
			}
			else
			{
				Position = PositionHelper.GetDetails(positionUID);
				Title = string.Format("Свойства должности: {0}", Position.Name);
			}
			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = Position.Name;
			Description = Position.Description;
			if (Position.Photo != null)
				PhotoData = Position.Photo.Data;
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

		byte[] _photoData;
		public byte[] PhotoData
		{
			get { return _photoData; }
			set
			{
				_photoData = value;
				OnPropertyChanged(() => PhotoData);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			Position.Name = Name;
			Position.Description = Description;
			if (Position.Photo == null)
				Position.Photo = new Photo();
			Position.Photo.Data = PhotoData;
			Position.OrganizationUID = Organization.UID;
			return PositionHelper.Save(Position);
		}
	}
}