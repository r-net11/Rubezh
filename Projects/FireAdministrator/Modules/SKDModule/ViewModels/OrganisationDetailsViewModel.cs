using System;
using System.Linq;
using StrazhAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationDetailsViewModel : SaveCancelDialogViewModel
	{
		readonly OrganisationsViewModel _organisationsViewModel;

		public OrganisationDetails OrganisationDetails { get; private set; }
		
		public bool IsNew { get; private set; }

		public OrganisationDetailsViewModel(OrganisationsViewModel organisationsViewModel, Organisation organisation = null)
		{
			_organisationsViewModel = organisationsViewModel;
			if (organisation == null)
			{
				IsNew = true;
				Title = "Создание новой организации";
				OrganisationDetails = new OrganisationDetails
				{
					Name = "Организация",
				};
				OrganisationDetails.UserUIDs.Add(FiresecManager.CurrentUser.UID);
			}
			else
			{
				Title = string.Format("Свойства организации: {0}", organisation.Name);
				OrganisationDetails = OrganisationHelper.GetDetails(organisation.UID);
			}
			CopyProperties();
		}

		private void CopyProperties()
		{
			Name = OrganisationDetails.Name;
			Description = OrganisationDetails.Description;
			Phone = OrganisationDetails.Phone;
			if (OrganisationDetails.Photo != null)
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
					Phone = OrganisationDetails.Phone,
					UserUIDs = OrganisationDetails.UserUIDs,
				};
			}
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		private string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		private byte[] _photoData;
		public byte[] PhotoData
		{
			get { return _photoData; }
			set
			{
				_photoData = value;
				OnPropertyChanged(() => PhotoData);
			}
		}

		private string _phone;
		public string Phone
		{
			get { return _phone; }
			set
			{
				if (_phone == value)
					return;
				_phone = value;
				OnPropertyChanged(() => Phone);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			if (_organisationsViewModel.Organisations.Any(x => x.Organisation.Name == Name && x.Organisation.UID != OrganisationDetails.UID && !x.Organisation.IsDeleted))
			{
				MessageBoxService.ShowWarning("Название организации совпадает с введенным ранее");
				return false;
			}

			OrganisationDetails.Name = Name;
			OrganisationDetails.Description = Description;
			if ((PhotoData != null && PhotoData.Length > 0) || OrganisationDetails.Photo != null)
			{
				OrganisationDetails.Photo = new Photo {Data = PhotoData};
			}
			OrganisationDetails.Phone = Phone;

			if (IsNew)
				OrganisationDetails.UserUIDs.Add(FiresecManager.CurrentUser.UID);
			return OrganisationHelper.Save(OrganisationDetails, IsNew);
		}
	}
}