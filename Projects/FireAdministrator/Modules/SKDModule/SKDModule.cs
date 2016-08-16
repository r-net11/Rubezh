using System;
using Infrastructure.Client;
using Infrastructure.Events;
using StrazhAPI.Enums;
using StrazhAPI.Models.Layouts;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Layouts;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using SKDModule.Validation;
using SKDModule.ViewModels;
using System.Collections.Generic;
using StrazhAPI.SKD;

namespace SKDModule
{
	public class SKDModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
	{
		private OrganisationsViewModel _organisationsViewModel;

		public override void CreateViewModels()
		{
			_organisationsViewModel = new OrganisationsViewModel();
		}

		public override void Initialize()
		{
			_organisationsViewModel.Initialize(LogicalDeletationType.Active);
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowOrganisationsEvent, Guid>(_organisationsViewModel, "Организации", "Kartoteka2W")
			};
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.SKD; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Layout/DataTemplates/Dictionary.xaml"));
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
			// Скрываем элемент "Верификация" в конфигураторе макетов интерфейса, если лицензия этого требует
			if (ServiceFactory.UiElementsVisibilityService.IsLayoutModuleVerificationElementVisible)
				yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDVerification, 304, "Верификация", "Панель верификация", "BTree.png") { Factory = (p) => new LayoutPartVerificationViewModel(p as LayoutPartReferenceProperties), };

			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.SKDHR, 305, "Картотека", "Панель картотека", "BLevels.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDDayIntervals, 306, "Дневные графики", "Панель дневные графики", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDScheduleSchemes, 307, "Графики", "Панель графики", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDHolidays, 308, "Праздничные дни", "Панель праздничные дни", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDSchedules, 309, "Графики работ", "Панель графики работ", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDTimeTracking, 310, "УРВ", "Панель учета рабочего времени", "BTree.png");
		}
		#endregion
	}
}