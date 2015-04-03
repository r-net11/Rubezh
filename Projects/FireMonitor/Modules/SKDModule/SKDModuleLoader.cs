using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.SKDReports;
using Infrastructure.Common.Windows;
using Infrastructure.Designer;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
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
				new NavigationItem("���", "SKDW",
					new List<NavigationItem>()
					{
						new NavigationItem<ShowHREvent>(HRViewModel, "���������", "Kartoteka2W"),
						new NavigationItem("���� �������� �������", "TimeTrackingW", new List<NavigationItem>()
						{
							new NavigationItem<ShowTimeIntervalsEvent, Guid>(DayIntervalsViewModel, "������� �������", "ShedulesDaylyW", null, PermissionType.Oper_SKD_TimeTrack_DaySchedules_View, Guid.Empty),
							new NavigationItem<ShowWeeklyIntervalsEvent, Guid>(ScheduleSchemesViewModel, "�������", "SheduleWeeklyW", null, PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_View, Guid.Empty),
							new NavigationItem<ShowHolidaysEvent, Guid>(HolidaysViewModel, "����������� ���", "HolidaysW", null, PermissionType.Oper_SKD_TimeTrack_Holidays_View, Guid.Empty),
							new NavigationItem<ShowShedulesEvent, Guid>(SchedulesViewModel, "������� �����", "ShedulesW", null, PermissionType.Oper_SKD_TimeTrack_Schedules_View, Guid.Empty),
							new NavigationItem<ShowTimeTrackingEvent>(TimeTrackingViewModel, "���� �������� �������", "TimeTrackingW", null, PermissionType.Oper_SKD_TimeTrack_Report_View),
						}),
					})
				};
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
		}

		#region ILayoutProviderModule Members
		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDHR, "���������", "Levels.png", (p) => HRViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDVerification, "�����������", "Tree.png", (p) => new VerificationViewModel(p as LayoutPartReferenceProperties));
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDDayIntervals, "������� �������", "Tree.png", (p) => DayIntervalsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDScheduleSchemes, "�������", "Tree.png", (p) => ScheduleSchemesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDHolidays, "����������� ���", "Tree.png", (p) => HolidaysViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDSchedules, "������� �����", "Tree.png", (p) => SchedulesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDTimeTracking, "����� �������� �������", "Tree.png", (p) => TimeTrackingViewModel);
		}
		#endregion

		#region ISKDReportProviderModule Members
		public IEnumerable<ISKDReportProvider> GetSKDReportProviders()
		{
			if (GlobalSettingsHelper.GlobalSettings.UseStrazhBrand)
			{
				yield return new EventsReportProvider();
				yield return new DoorsReportProvider();
			}
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