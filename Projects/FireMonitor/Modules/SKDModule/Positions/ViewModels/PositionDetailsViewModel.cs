using System;
using Localization.SKD.ViewModels;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class PositionDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<ShortPosition>
	{
		public Position Position { get; private set; }
		public ShortPosition Model
		{
			get
			{
				return new ShortPosition
				{
					UID = Position.UID,
					Name = Position.Name,
					Description = Position.Description,
					OrganisationUID = Position.OrganisationUID
				};
			}
		}

		Guid OrganisationUID { get; set; }
		bool _isNew;

		public PositionDetailsViewModel() { }

		public void Initialize(Guid orgnaisationUID, Guid? positionUID = null)
		{
			OrganisationUID = orgnaisationUID;
			_isNew = positionUID == null;
			if (_isNew)
			{
				Title = CommonViewModels.CreatePosition;
				Position = new Position()
				{
					Name = CommonViewModels.NewPosition,
					OrganisationUID = OrganisationUID
				};
			}
			else
			{
				Position = PositionHelper.GetDetails(positionUID);
				Title = string.Format(CommonViewModels.PositionProperties, Position.Name);
			}
			CopyProperties();
		}

		public bool Initialize(Organisation organisation, ShortPosition model, ViewPartViewModel parentViewModel)
		{
			Initialize(organisation.UID, model != null ? (Guid?)model.UID : null);
			return true;
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
					OnPropertyChanged(() => Name);
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
					OnPropertyChanged(() => Description);
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
			Position.OrganisationUID = OrganisationUID;

			return PositionHelper.Save(Position, _isNew);
		}
	}
}