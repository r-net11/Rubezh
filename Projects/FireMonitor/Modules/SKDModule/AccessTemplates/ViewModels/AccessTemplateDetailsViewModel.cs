using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class AccessTemplateDetailsViewModel : SaveCancelDialogViewModel
	{
		Organisation Organisation { get; set; }
		public AccessTemplate AccessTemplate { get; private set; }
		public AccessDoorsSelectationViewModel AccessDoorsSelectationViewModel { get; private set; }

		public AccessTemplateDetailsViewModel(Organisation orgnaisation, AccessTemplate accessTemplate = null)
		{
			Organisation = orgnaisation;
			if (accessTemplate == null)
			{
				Title = "Создание шаблона доступа";
				accessTemplate = new AccessTemplate()
				{
					Name = "Новый шаблон доступа",
				};
			}
			else
			{
				Title = string.Format("Свойства шаблона доступа: {0}", accessTemplate.Name);
			}
			AccessTemplate = accessTemplate;
			AccessTemplateGuardZones = new AccessTemplateGuardZonesViewModel(AccessTemplate);
			CopyProperties();
			AccessDoorsSelectationViewModel = new AccessDoorsSelectationViewModel(Organisation, AccessTemplate.CardDoors, AccessTemplate.UID);
		}

		public void CopyProperties()
		{
			Name = AccessTemplate.Name;
			Description = AccessTemplate.Description;
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

		public AccessTemplateGuardZonesViewModel AccessTemplateGuardZones { get; private set; }

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			if (AccessTemplate.Name == "НЕТ")
			{
				MessageBoxService.ShowWarning("Запрещенное название");
				return false;
			}

			AccessTemplate.Name = Name;
			AccessTemplate.Description = Description;
			AccessTemplate.CardDoors = AccessDoorsSelectationViewModel.GetCardDoors();
			AccessTemplate.OrganisationUID = Organisation.UID;
			return AccessTemplateHelper.Save(AccessTemplate);
		}
	}
}