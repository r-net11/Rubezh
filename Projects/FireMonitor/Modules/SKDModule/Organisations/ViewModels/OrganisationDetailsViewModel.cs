using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationDetailsViewModel : SaveCancelDialogViewModel
	{
		OrganisationsViewModel OrganisationsViewModel;
		public OrganisationDetails OrganisationDetails { get; private set; }
		public ChiefViewModel ChiefViewModel { get; private set; }
		public ChiefViewModel HRChiefViewModel { get; private set; }
		public bool IsNew { get; private set; }

		public OrganisationDetailsViewModel(OrganisationsViewModel organisationsViewModel, Organisation organisation = null)
		{
			OrganisationsViewModel = organisationsViewModel;
			if (organisation == null)
			{
				IsNew = true;
				Title = "Создание новой организации";
				OrganisationDetails = new OrganisationDetails()
				{
					Name = "Огранизация",
				};
			}
			else
			{
				Title = string.Format("Свойства организации: {0}", organisation.Name);
				OrganisationDetails = OrganisationHelper.GetDetails(organisation.UID);
			}
			CopyProperties();
			ChiefViewModel = new ChiefViewModel(OrganisationDetails.ChiefUID, new EmployeeFilter { OrganisationUIDs = new List<Guid> { OrganisationDetails.UID } });
			HRChiefViewModel = new ChiefViewModel(OrganisationDetails.HRChiefUID, new EmployeeFilter { OrganisationUIDs = new List<Guid> { OrganisationDetails.UID } });
		}

		void CopyProperties()
		{
			Name = OrganisationDetails.Name;
			Description = OrganisationDetails.Description;
			Phone = OrganisationDetails.Phone;
			if(OrganisationDetails.Photo != null)
				PhotoData = OrganisationDetails.Photo.Data;
		}

		public Organisation Organisation
		{
			get
			{
				return new Organisation
				{
					Description = OrganisationDetails.Description,
					IsDeleted = OrganisationDetails.IsDeleted,
					Name = OrganisationDetails.Name,
					PhotoUID = OrganisationDetails.Photo != null ? OrganisationDetails.Photo.UID : Guid.Empty,
					RemovalDate = OrganisationDetails.RemovalDate,
					UID = OrganisationDetails.UID,
					DoorUIDs = OrganisationDetails.DoorUIDs,
					Phone = OrganisationDetails.Phone
				};
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

		string _phone;
		public string Phone
		{
			get { return _phone; }
			set
			{
				if (_phone != value)
				{
					_phone = value;
					OnPropertyChanged(() => Phone);
				}
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			if (OrganisationsViewModel.Organisations.Any(x => x.Organisation.Name == Name && x.Organisation.UID != OrganisationDetails.UID))
			{
				MessageBoxService.ShowWarning("Название организации совпадает с введенным ранее");
				return false;
			}

			OrganisationDetails.Name = Name;
			OrganisationDetails.Description = Description;
			if (PhotoData != null && PhotoData.Length > 0)
			{
				OrganisationDetails.Photo = new Photo();
				OrganisationDetails.Photo.Data = PhotoData;
			}
			OrganisationDetails.ChiefUID = ChiefViewModel.ChiefUID;
			OrganisationDetails.HRChiefUID = HRChiefViewModel.ChiefUID;
			OrganisationDetails.Phone = Phone;
			if (Validate())
				return OrganisationHelper.Save(OrganisationDetails, IsNew);
			else
				return false;
		}

		bool Validate()
		{
			if (OrganisationDetails.Name != null && OrganisationDetails.Name.Length > 50)
			{
				MessageBoxService.Show("Значение поля 'Название' не может быть длиннее 50 символов");
				return false;
			}
			if (OrganisationDetails.Description != null && OrganisationDetails.Description.Length > 50)
			{
				MessageBoxService.Show("Значение поля 'Описание' не может быть длиннее 4000 символов");
				return false;
			}
			return true;
		}
	}
}