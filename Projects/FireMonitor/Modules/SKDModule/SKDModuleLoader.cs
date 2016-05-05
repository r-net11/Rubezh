using System;
using System.Collections.Generic;
using FiresecAPI.Enums;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Layouts;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.SKDReports;
using Infrastructure.Common.Windows;
using Infrastructure.Designer;
using Infrastructure.Events;
using SKDModule.Events;
using SKDModule.Reports;
using SKDModule.Reports.Providers;
using SKDModule.ViewModels;

namespace SKDModule
{
	public class SKDModuleLoader : ModuleBase, IReportProviderModule, ILayoutProviderModule, ISKDReportProviderModule
	{
		HRViewModel HRViewModel;
		DayIntervalsViewModel DayIntervalsViewModel;
		ScheduleSchemesViewModel ScheduleSchemesViewModel;
		HolidaysViewModel HolidaysViewModel;
		SchedulesViewModel SchedulesViewModel;
		TimeTrackingViewModel TimeTrackingViewModel;

		public override void CreateViewModels()
		{
			HRViewModel = new HRViewModel();
			DayIntervalsViewModel = new DayIntervalsViewModel();
			ScheduleSchemesViewModel = new ScheduleSchemesViewModel();
			HolidaysViewModel = new HolidaysViewModel();
			SchedulesViewModel = new SchedulesViewModel();
			TimeTrackingViewModel = new TimeTrackingViewModel();
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>
			{
				BuildSkdItem()
			};
		}

		private NavigationItem BuildSkdItem()
		{
			var items = new List<NavigationItem>
			{
				new NavigationItem<ShowHREvent>(HRViewModel, "Картотека", "Kartoteka2W")
			};

			if (ServiceFactory.UiElementsVisibilityService.IsMainMenuSkdUrvElementVisible)
				items.Add(BuildUrvItem());

			return new NavigationItem("СКД", "SKDW", items);
		}

		private NavigationItem BuildUrvItem()
		{
			return new NavigationItem("Учет рабочего времени", "TimeTrackingW", new List<NavigationItem>()
			{
				new NavigationItem<ShowTimeIntervalsEvent, Guid>(DayIntervalsViewModel, "Дневные графики", "ShedulesDaylyW", null,
					PermissionType.Oper_SKD_TimeTrack_DaySchedules_View, Guid.Empty),
				new NavigationItem<ShowWeeklyIntervalsEvent, Guid>(ScheduleSchemesViewModel, "Графики", "SheduleWeeklyW", null,
					PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_View, Guid.Empty),
				new NavigationItem<ShowHolidaysEvent, Guid>(HolidaysViewModel, "Праздничные дни", "HolidaysW", null,
					PermissionType.Oper_SKD_TimeTrack_Holidays_View, Guid.Empty),
				new NavigationItem<ShowShedulesEvent, Guid>(SchedulesViewModel, "График работы", "ShedulesW", null,
					PermissionType.Oper_SKD_TimeTrack_Schedules_View, Guid.Empty),
				new NavigationItem<ShowTimeTrackingEvent>(TimeTrackingViewModel, "Учет рабочего времени", "TimeTrackingW", null,
					PermissionType.Oper_SKD_TimeTrack_Report_View),
			});
		}

		public override void Initialize()
		{
			DayIntervalsViewModel.Initialize();
			ScheduleSchemesViewModel.Initialize();
			HolidaysViewModel.Initialize();
			SchedulesViewModel.Initialize();
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.SKD; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "HR/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Employees/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Departments/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Positions/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "AdditionalColumns/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "AccessTemplates/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Organisations/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Verification/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Cards/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "TimeTrack/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "PassCard/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "PassCardDesigner/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Reports/DataTemplates/Dictionary.xaml"));
			DesignerLoader.RegisterResource();
		}

		#region IReportProviderModule Members
		public IEnumerable<IReportProvider> GetReportProviders()
		{
			return new List<IReportProvider>()
			{
				new DeviceListReport(),
			};
		}
		#endregion

		public override bool BeforeInitialize(bool firstTime)
		{
			return true;
		}
		
		public override void AfterInitialize()
		{
			// Отслеживаем события прохода по "Гостевой" карте
			SafeFiresecService.GuestCardPassedEvent -= SafeFiresecServiceOnGuestCardPassedEvent;
			SafeFiresecService.GuestCardPassedEvent += SafeFiresecServiceOnGuestCardPassedEvent;

			// Отслеживаем события деактивации карты
			SafeFiresecService.CardDeactivatedEvent -= SafeFiresecServiceOnCardDeactivatedEvent;
			SafeFiresecService.CardDeactivatedEvent += SafeFiresecServiceOnCardDeactivatedEvent;
		}

		private void SafeFiresecServiceOnCardDeactivatedEvent(SKDCard card)
		{
			ApplicationService.Invoke(() => ServiceFactoryBase.Events.GetEvent<CardDeactivatedEvent>().Publish(card));
		}

		private void SafeFiresecServiceOnGuestCardPassedEvent(SKDCard card)
		{
			ApplicationService.Invoke(() => ServiceFactoryBase.Events.GetEvent<GuestCardPassedEvent>().Publish(card));
		}

		#region ILayoutProviderModule Members
		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDHR, "Картотека", "Levels.png", (p) => HRViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDVerification, "Верификация", "Tree.png", (p) => new VerificationViewModel(p as LayoutPartReferenceProperties));
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDDayIntervals, "Дневные графики", "Tree.png", (p) => DayIntervalsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDScheduleSchemes, "Графики", "Tree.png", (p) => ScheduleSchemesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDHolidays, "Праздничные дни", "Tree.png", (p) => HolidaysViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDSchedules, "Графики работ", "Tree.png", (p) => SchedulesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDTimeTracking, "Учет рабочего времени", "Tree.png", (p) => TimeTrackingViewModel);
		}
		#endregion

		#region ISKDReportProviderModule Members
		public IEnumerable<ISKDReportProvider> GetSKDReportProviders()
		{
			yield return new EventsReportProvider();
			yield return new DoorsReportProvider();
			yield return new EmployeeRootReportProvider();
			yield return new CardsReportProvider();
			yield return new EmployeeAccessReportProvider();
			yield return new EmployeeDoorsReportProvider();
			yield return new DepartmentsReportProvider();
			yield return new PositionsReportProvider();
			yield return new EmployeeZonesReportProvider();
			yield return new EmployeeReportProvider();
			yield return new DisciplineReportProvider();
			yield return new SchedulesReportProvider();
			yield return new DocumentsReportProvider();
			yield return new WorkingTimeReportProvider();
		}
		#endregion
	}
}