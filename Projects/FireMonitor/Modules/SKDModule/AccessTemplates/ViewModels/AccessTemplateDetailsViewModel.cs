using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class AccessTemplateDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<AccessTemplate>
	{
		Organisation Organisation { get; set; }
		public AccessTemplate Model { get; private set; }
		public AccessDoorsSelectationViewModel AccessDoorsSelectationViewModel { get; private set; }
		bool _isNew;
		public AccessTemplateDetailsViewModel() {  }
		
		public bool Initialize(Organisation orgnaisation, AccessTemplate accessTemplate, ViewPartViewModel parentViewModel)
		{
			Organisation = OrganisationHelper.GetSingle(orgnaisation.UID); ;
			_isNew = accessTemplate == null;
			if (_isNew)
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
			Model = accessTemplate;
			CopyProperties();
			AccessDoorsSelectationViewModel = new AccessDoorsSelectationViewModel(Organisation, Model.CardDoors, GKScheduleHelper.GetSchedules());
			return true;
		}

		public void CopyProperties()
		{
			Name = Model.Name;
			Description = Model.Description;
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

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			if (Model.Name == "НЕТ")
			{
				MessageBoxService.ShowWarning("Запрещенное название");
				return false;
			}

			Model.Name = Name;
			Model.Description = Description;
			Model.CardDoors = AccessDoorsSelectationViewModel.GetCardDoors();
			Model.CardDoors.ForEach(x => x.AccessTemplateUID = Model.UID);
			Model.OrganisationUID = Organisation.UID;
			if (!DetailsValidateHelper.Validate(Model))
				return false;
			var saveResult = AccessTemplateHelper.Save(Model, _isNew);
			if(saveResult)
			{
				ServiceFactory.Events.GetEvent<UpdateAccessTemplateEvent>().Publish(Model.UID);
				return true;
			}
			return false;
		}
	}
}