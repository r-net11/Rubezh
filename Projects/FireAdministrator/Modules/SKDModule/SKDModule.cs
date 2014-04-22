using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using Infrastructure.Designer;
using Infrustructure.Plans.Events;
using SKDModule.Events;
using SKDModule.PassCard.ViewModels;
using SKDModule.Plans;
using SKDModule.Plans.Designer;
using SKDModule.Validation;
using SKDModule.ViewModels;
using FiresecAPI.Models.Layouts;

namespace SKDModule
{
	public class SKDModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
	{
		DevicesViewModel DevicesViewModel;
		ZonesViewModel ZonesViewModel;
		LibraryViewModel LibraryViewModel;
		TimeIntervalsViewModel TimeIntervalsViewModel;
		WeeklyIntervalsViewModel WeeklyIntervalsViewModel;
		SlideDayIntervalsViewModel SlideDayIntervalsViewModel;
		SlideWeekIntervalsViewModel SlideWeekIntervalsViewModel;
		HolidaysViewModel HolidaysViewModel;
		PassCardsDesignerViewModel PassCardDesignerViewModel;
		SettingsViewModel SettingsViewModel;
		FiltersViewModel FiltersViewModel;
		OrganisationsViewModel OrganisationsViewModel;
		SKDPlanExtension _planExtension;

		public override void CreateViewModels()
		{
			SKDManager.UpdateConfiguration();
			ServiceFactory.Events.GetEvent<CreateSKDZoneEvent>().Subscribe(OnCreateSKDZone);
			ServiceFactory.Events.GetEvent<EditSKDZoneEvent>().Subscribe(OnEditSKDZone);

			PassCardDesignerViewModel = new PassCardsDesignerViewModel();
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			LibraryViewModel = new LibraryViewModel();
			TimeIntervalsViewModel = new TimeIntervalsViewModel();
			WeeklyIntervalsViewModel = new WeeklyIntervalsViewModel();
			SlideDayIntervalsViewModel = new SlideDayIntervalsViewModel();
			SlideWeekIntervalsViewModel = new SlideWeekIntervalsViewModel();
			HolidaysViewModel = new HolidaysViewModel();
			SettingsViewModel = new SettingsViewModel();
			FiltersViewModel = new FiltersViewModel();
			OrganisationsViewModel = new OrganisationsViewModel();
			_planExtension = new SKDPlanExtension(DevicesViewModel, ZonesViewModel);
		}

		public override void Initialize()
		{
			PassCardDesignerViewModel.Initialize();
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
			FiltersViewModel.Initialize();
			OrganisationsViewModel.Initialize();

			TimeIntervalsViewModel.Initialize();
			WeeklyIntervalsViewModel.Initialize();
			SlideDayIntervalsViewModel.Initialize();
			SlideWeekIntervalsViewModel.Initialize();
			HolidaysViewModel.Initialize();

			_planExtension.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
			Helper.BuildMap();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			if (!FiresecManager.CheckPermission(PermissionType.Adm_SKUD))
				return null;

			return new List<NavigationItem>()
			{
				new NavigationItem("СКД", null, new List<NavigationItem>()
				{
					new NavigationItem<ShowSKDDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/Tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowSKDZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/Tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowSKDLidraryEvent, object>(LibraryViewModel, "Библиотека", "/Controls;component/Images/Book.png"),
					new NavigationItem<ShowSKDFiltersEvent, Guid>(FiltersViewModel, "Фильтры", "/Controls;component/Images/Book.png", null, null, Guid.Empty),
					new NavigationItem<ShowSKDOrganisationsEvent, Guid>(OrganisationsViewModel, "Организации", "/Controls;component/Images/Book.png", null, null, Guid.Empty),
					new NavigationItem<ShowPassCardDesignerEvent, Guid>(PassCardDesignerViewModel, "Дизайнер пропусков",null,null,null, Guid.Empty),
					new NavigationItem<ShowSKDSettingsEvent, object>(SettingsViewModel, "Настройки", "/Controls;component/Images/Book.png"),
					new NavigationItem("Интервалы", null, new List<NavigationItem>()
					{
						new NavigationItem<ShowSKDTimeIntervalsEvent, Guid>(TimeIntervalsViewModel, "Именованные интервалы", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
						new NavigationItem<ShowSKDWeeklyIntervalsEvent, Guid>(WeeklyIntervalsViewModel, "Недельные графики", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
						new NavigationItem<ShowSKDSlideDayIntervalsEvent, Guid>(SlideDayIntervalsViewModel, "Скользящие посуточные графики", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
						new NavigationItem<ShowSKDSlideWeekIntervalsEvent, Guid>(SlideWeekIntervalsViewModel, "Скользящие понедельные графики", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
						new NavigationItem<ShowSKDHolidaysEvent, Guid>(HolidaysViewModel, "Праздничные дни", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
					}),
				}, PermissionType.Adm_SKUD) {IsExpanded = true},
			};
		}
		public override string Name
		{
			get { return "СКД"; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Devices/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Zones/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Library/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Filters/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Organisations/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "PassCard/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Intervals/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Settings/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Layout/DataTemplates/Dictionary.xaml"));
			DesignerLoader.RegisterResource();
		}

		#region IValidationModule Members
		public IEnumerable<IValidationError> Validate()
		{
			var validator = new Validator();
			return validator.Validate();
		}
		#endregion

		private void OnCreateSKDZone(CreateSKDZoneEventArg createZoneEventArg)
		{
			ZonesViewModel.CreateZone(createZoneEventArg);
		}
		private void OnEditSKDZone(Guid zoneUID)
		{
			ZonesViewModel.EditZone(zoneUID);
		}

		#region ILayoutDeclarationModule Members

		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDDevices, 303, "СКД устройства", "Панель СКД устройства", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDZones, 304, "СКД зоны", "Панель СКД зоны", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDJournal, 302, "Журнал", "Панель журнал", "BLevels.png") { Factory = (p) => new LayoutPartJournalViewModel(p as LayoutPartSKDJournalProperties), };
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDVerification, 305, "Верификация", "Панель верификация", "BTree.png") { Factory = (p) => new LayoutPartVerificationViewModel(p as LayoutPartSKDVerificationProperties), };
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDEmployees, 301, "Сотрудники", "Панель сотрудники", "BLevels.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDCards, 306, "Карты", "Панель карты", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDDepartments, 307, "Отделы", "Панель отделы", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDPositions, 308, "Должности", "Панель должности", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDDocuments, 39, "Документы", "Панель документы", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDAdditionalColumns, 310, "Дополнительные колонки", "Панель дополнительные колонки", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDNamedIntervals, 311, "Именованные интервалы", "Панель именованные интервалы", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDWeeklyScheduleSchemes, 312, "Недельные графики", "Панель недельные графики", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDDaylyScheduleSchemes, 313, "Скользящие посуточные графики", "Панель скользящие посуточные графики", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDMonthlyScheduleSchemes, 314, "Месячные графики", "Панель месячные графики", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDHolidays, 315, "Праздничные дни", "Панель праздничные дни", "BTree.png");
		}

		#endregion
	}
}