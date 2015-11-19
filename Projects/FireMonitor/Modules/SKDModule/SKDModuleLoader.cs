using System.Collections.Generic;
using RubezhAPI.Models.Layouts;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.SKDReports;
using Infrastructure.Designer;
using SKDModule.Events;
using SKDModule.Reports;
using SKDModule.Reports.Providers;
using SKDModule.ViewModels;
using RubezhClient;
using RubezhAPI.License;
using RubezhAPI.Models;
using System.Linq;
namespace SKDModule
{
	public class SKDModuleLoader : ModuleBase, IReportProviderModule, ILayoutProviderModule, ISKDReportProviderModule
	{
		SKDTabItems SKDTabItems; 
		
		public override void CreateViewModels()
		{
			SKDTabItems = new SKDTabItems();
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			IEnumerable<PermissionType> cardFielsPernissionType = new List<PermissionType>()
			{
				PermissionType.Oper_SKD_Employees_View,
				PermissionType.Oper_SKD_Guests_View,
				PermissionType.Oper_SKD_Departments_View,
				PermissionType.Oper_SKD_Positions_View,
				PermissionType.Oper_SKD_AdditionalColumns_View,
				PermissionType.Oper_SKD_Cards_View,
				PermissionType.Oper_SKD_AccessTemplates_View,
				PermissionType.Oper_SKD_PassCards_View,
				PermissionType.Oper_SKD_Organisations_View,
			};

			IEnumerable<PermissionType> timeTrackingPernissionType = new List<PermissionType>()
			{
				PermissionType.Oper_SKD_TimeTrack_DaySchedules_View,
				PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_View,
				PermissionType.Oper_SKD_TimeTrack_Holidays_View,
				PermissionType.Oper_SKD_TimeTrack_Schedules_View,
				PermissionType.Oper_SKD_TimeTrack_Report_View
			};
			bool isTimeTracking = timeTrackingPernissionType.Any(x=> ClientManager.CheckPermission(x));
			bool isCardFiels = cardFielsPernissionType.Any(x => ClientManager.CheckPermission(x));
			return new List<NavigationItem>
				{
				new NavigationItem("СКД", "SKDW",
					new List<NavigationItem>()
					{
						new NavigationItem<ShowHREvent>(SKDTabItems.HRViewModel, "Картотека", "Kartoteka2W"){IsVisible = isCardFiels},
						new NavigationItem<ShowTimeTrackingEvent>(SKDTabItems.TimeTrackingTabsViewModel, "Учет рабочего времени", "TimeTrackingW"){IsVisible = isTimeTracking}
					}) { IsVisible = LicenseManager.CurrentLicenseInfo.HasSKD && (isCardFiels || isTimeTracking)}
				};
		}

		public override void Initialize()
		{
			SKDTabItems.Initialize();
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

		#region ILayoutProviderModule Members
		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDHR, "Картотека", "Levels.png", (p) => SKDTabItems.HRViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDVerification, "Верификация", "Tree.png", (p) => new VerificationViewModel(p as LayoutPartReferenceProperties));
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDTimeTracking, "Учет рабочего времени", "Tree.png", (p) => SKDTabItems.TimeTrackingTabsViewModel);
		}
		#endregion

		#region ISKDReportProviderModule Members
		public IEnumerable<ISKDReportProvider> GetSKDReportProviders()
		{
			yield return new EventsReportProvider();
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

public class SKDTabItems
{
	public HRFilter Filter { get; set; }
	public HRViewModel HRViewModel { get; set; }
	public TimeTrackingTabsViewModel TimeTrackingTabsViewModel { get; set; }
	public SKDTabItems()
	{
		var userUID = ClientManager.CurrentUser.UID;
		Filter = new HRFilter() { UserUID = ClientManager.CurrentUser.UID };
		Filter.EmployeeFilter.UserUID = userUID;
		HRViewModel = new HRViewModel(this);
		TimeTrackingTabsViewModel = new TimeTrackingTabsViewModel(this);
	}

	public void Initialize()
	{
		HRViewModel.Initialize();
		TimeTrackingTabsViewModel.Initialize();
	}
}