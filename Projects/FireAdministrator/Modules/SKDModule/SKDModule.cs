using System;
using System.Collections.Generic;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using Infrustructure.Plans.Events;
using SKDModule.Validation;
using SKDModule.ViewModels;
using RubezhAPI.SKD;
using Infrastructure.Events;
using System.Linq;

namespace SKDModule
{
	public class SKDModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
	{
		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<SelectOrganisationEvent>().Subscribe(OnSelectOrganisation);
			ServiceFactory.Events.GetEvent<SelectOrganisationsEvent>().Subscribe(OnSelectOrganisations);
		}

		public override void Initialize()
		{
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
			};
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.SKD; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Layout/DataTemplates/Dictionary.xaml");
		}

		void OnSelectOrganisation(SelectOrganisationEventArg selectOrganisationEventArg)
		{
			var cameraSelectionViewModel = new OrganisationSelectionViewModel(selectOrganisationEventArg.Organisation);
			selectOrganisationEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(cameraSelectionViewModel);
			selectOrganisationEventArg.Organisation = selectOrganisationEventArg.Cancel || cameraSelectionViewModel.SelectedOrganisation == null ?
				null :
				cameraSelectionViewModel.SelectedOrganisation.Organisation;
		}
		void OnSelectOrganisations(SelectOrganisationsEventArg selectOrganisationsEventArg)
		{
			var organisationsSelectionViewModel = new OrganisationsSelectionViewModel(selectOrganisationsEventArg.Organisations);
			selectOrganisationsEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(organisationsSelectionViewModel);
			selectOrganisationsEventArg.Organisations = organisationsSelectionViewModel.TargetOrganisations.ToList();
		}

		#region IValidationModule Members
		public IEnumerable<IValidationError> Validate()
		{
			var validator = new Validator();
			return validator.Validate();
		}
		#endregion

		#region ILayoutDeclarationModule Members
		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDVerification, 304, "Верификация", "Панель верификация", "BTree.png") { Factory = (p) => new LayoutPartVerificationViewModel(p as LayoutPartReferenceProperties), };
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.SKDHR, 305, "Картотека", "Панель картотека", "BLevels.png", false);
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDTimeTracking, 310, "УРВ", "Панель учета рабочеговремени", "BTree.png", false);
		}
		#endregion
	}
}