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

		public OrganisationDetailsViewModel(OrganisationsViewModel organisationsViewModel, Organisation organisation = null)
		{
			OrganisationsViewModel = organisationsViewModel;
			if (organisation == null)
			{
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
		}

		void CopyProperties()
		{
			Name = OrganisationDetails.Name;
			Description = OrganisationDetails.Description;
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
					PhotoUID = OrganisationDetails.Photo.UID,
					RemovalDate = OrganisationDetails.RemovalDate,
					UID = OrganisationDetails.UID,
					ZoneUIDs = OrganisationDetails.ZoneUIDs
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
			return OrganisationHelper.Save(OrganisationDetails);
		}
	}
}