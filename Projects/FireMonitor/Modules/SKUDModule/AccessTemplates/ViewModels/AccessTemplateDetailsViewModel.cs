using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class AccessTemplateDetailsViewModel : SaveCancelDialogViewModel
	{
		Organisation Organisation { get; set; }
		public AccessTemplate AccessTemplate { get; private set; }
		public AccessZonesSelectationViewModel AccessZonesSelectationViewModel { get; private set; }

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
			CopyProperties();
			AccessZonesSelectationViewModel = new AccessZonesSelectationViewModel(Organisation, AccessTemplate.CardZones, AccessTemplate.UID);
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
			AccessTemplate.CardZones = AccessZonesSelectationViewModel.GetCardZones();
			AccessTemplate.OrganisationUID = Organisation.UID;
			return AccessTemplateHelper.Save(AccessTemplate);
		}
	}
}