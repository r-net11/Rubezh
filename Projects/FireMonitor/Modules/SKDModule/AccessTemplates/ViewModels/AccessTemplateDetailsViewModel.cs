using System.Windows.Navigation;
using Localization.SKD.ViewModels;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class AccessTemplateDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<AccessTemplate>
	{
		Organisation Organisation { get; set; }
		public AccessTemplate Model { get; private set; }
		public AccessDoorsSelectationViewModel AccessDoorsSelectationViewModel { get; private set; }
		public DeactivatingReadersSelectationViewModel DeactivatingReadersSelectationViewModel { get; private set; }
		bool _isNew;
		
		public AccessTemplateDetailsViewModel() {  }
		
		public bool Initialize(Organisation orgnaisation, AccessTemplate accessTemplate, ViewPartViewModel parentViewModel)
		{
			Organisation = OrganisationHelper.GetSingle(orgnaisation.UID);
			_isNew = accessTemplate == null;
			if (_isNew)
			{
				Title = CommonViewModels.CreateAccessTempl;
				accessTemplate = new AccessTemplate {Name = CommonViewModels.NewAccessTemplate};
			}
			else if (accessTemplate != null)
				Title = string.Format(CommonViewModels.AccessTemplProperties, accessTemplate.Name);

			Model = accessTemplate;
			CopyProperties();
			AccessDoorsSelectationViewModel = new AccessDoorsSelectationViewModel(Organisation, Model.CardDoors);
			DeactivatingReadersSelectationViewModel = new DeactivatingReadersSelectationViewModel(AccessDoorsSelectationViewModel.GetSelectedDoorsUids(), Model.DeactivatingReaders);
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
				if (_name == value) return;
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
			if (Model.Name == CommonViewModels.NO)
			{
				MessageBoxService.ShowWarning(CommonViewModels.ForbiddenName);
				return false;
			}

			Model.Name = Name;
			Model.Description = Description;
			
			Model.CardDoors = AccessDoorsSelectationViewModel.GetCardDoors();
			Model.CardDoors.ForEach(x => x.AccessTemplateUID = Model.UID);
			
			Model.DeactivatingReaders = DeactivatingReadersSelectationViewModel.GetReaders();
			Model.DeactivatingReaders.ForEach(x => x.AccessTemplateUID = Model.UID);
			
			Model.OrganisationUID = Organisation.UID;

			if (!AccessTemplateHelper.Save(Model, _isNew)) return false;

			ServiceFactoryBase.Events.GetEvent<UpdateAccessTemplateEvent>().Publish(Model.UID);
			return true;
		}
	}
}