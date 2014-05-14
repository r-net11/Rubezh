using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationDetailsViewModel : SaveCancelDialogViewModel
	{
		OrganisationsViewModel OrganisationsViewModel;
		bool IsNew;
		public Organisation Organisation { get; set; }

		public OrganisationDetailsViewModel(OrganisationsViewModel organisationsViewModel, Organisation organisation = null)
		{
			OrganisationsViewModel = organisationsViewModel;
			if (organisation == null)
			{
				Title = "Создание новой организации";

				Organisation = new Organisation()
				{
					Name = "Огранизация",
				};
			}
			else
			{
				Title = string.Format("Свойства организации: {0}", organisation.Name);
				Organisation = organisation;
			}
			CopyProperties();
		}

		void CopyProperties()
		{
			Name = Organisation.Name;
			Description = Organisation.Description;
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

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			if (OrganisationsViewModel.Organisations.Any(x => x.Organisation.Name == Name && x.Organisation.UID != Organisation.UID))
			{
				MessageBoxService.ShowWarning("Название организации совпадает с введенным ранее");
				return false;
			}

			Organisation.Name = Name;
			Organisation.Description = Description;
			return base.Save();
		}
	}
}