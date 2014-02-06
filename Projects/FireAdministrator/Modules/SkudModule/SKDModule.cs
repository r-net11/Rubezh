using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using SKDModule.ViewModels;
using System;
using System.Linq;
using XFiresecAPI;
using FiresecAPI;
using SKDModule.Plans;
using Infrustructure.Plans.Events;
using SKDModule.Plans.Designer;

namespace SKDModule
{
	public class SKDModule : ModuleBase
	{
		SkudViewModel _skdViewModel;
		//EmployeeCardIndexViewModel _employeeCardIndexViewModel;
		//EmployeeDepartmentsViewModel _employeeDepartmentsViewModel;
		//EmployeePositionsViewModel _employeePositionsViewModel;
		//EmployeeGroupsViewModel _employeeGroupsViewModel;
		PassCardsDesignerViewModel _passCardDesignerViewModel;
		DevicesViewModel DevicesViewModel;
		ZonesViewModel ZonesViewModel;
		LibraryViewModel LibraryViewModel;
		TimeIntervalsViewModel TimeIntervalsViewModel;
		SlideDayIntervalsViewModel SlideDayIntervalsViewModel;
		SlideWeekIntervalsViewModel SlideWeekIntervalsViewModel;
		WeeklyIntervalsViewModel WeeklyIntervalsViewModel;
		HolidaysViewModel HolidaysViewModel;
		SettingsViewModel SettingsViewModel;
		FiltersViewModel FiltersViewModel;
		SKDPlanExtension _planExtension;

		public override void CreateViewModels()
		{
			SKDManager.UpdateConfiguration();

			ServiceFactory.Events.GetEvent<ShowSKDEvent>().Unsubscribe(OnShowSKD);
			ServiceFactory.Events.GetEvent<ShowSKDEvent>().Subscribe(OnShowSKD);
			ServiceFactory.Events.GetEvent<CreateSKDZoneEvent>().Subscribe(OnCreateSKDZone);
			ServiceFactory.Events.GetEvent<EditSKDZoneEvent>().Subscribe(OnEditSKDZone);

			_skdViewModel = new SkudViewModel();
			//_employeeCardIndexViewModel = new EmployeeCardIndexViewModel();
			//_employeeDepartmentsViewModel = new EmployeeDepartmentsViewModel();
			//_employeeGroupsViewModel = new EmployeeGroupsViewModel();
			//_employeePositionsViewModel = new EmployeePositionsViewModel();
			_passCardDesignerViewModel = new PassCardsDesignerViewModel();
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			LibraryViewModel = new LibraryViewModel();
			TimeIntervalsViewModel = new TimeIntervalsViewModel();
			SlideDayIntervalsViewModel = new SlideDayIntervalsViewModel();
			SlideWeekIntervalsViewModel = new SlideWeekIntervalsViewModel();
			WeeklyIntervalsViewModel = new WeeklyIntervalsViewModel();
			HolidaysViewModel = new HolidaysViewModel();
			SettingsViewModel = new SettingsViewModel();
			FiltersViewModel = new FiltersViewModel();
			_planExtension = new SKDPlanExtension(DevicesViewModel, ZonesViewModel);
		}

		private void OnShowSKD(object obj)
		{
			//ServiceFactory.Layout.Show(_skudViewModel);
		}

		public override void Initialize()
		{
			_skdViewModel.Initialize();
			//_employeeCardIndexViewModel.Initialize();

			//_employeeDepartmentsViewModel.Initialize();
			//_employeeGroupsViewModel.Initialize();
			//_employeePositionsViewModel.Initialize();
			_passCardDesignerViewModel.Initialize();
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
			FiltersViewModel.Initialize();
		
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
				new NavigationItem("СКУД", null, new List<NavigationItem>()
				{
					//new NavigationItem<ShowEmployeeCardIndexEvent>(_employeeCardIndexViewModel, "Картотека",null),
					new NavigationItem<ShowPassCardEvent>(_skdViewModel, "Пропуск",null),
					new NavigationItem<ShowPassCardDesignerEvent>(_passCardDesignerViewModel, "Дизайнер",null),
					//new NavigationItem("Справочники",null, new List<NavigationItem>()
					//{
					//    new NavigationItem<ShowEmployeePositionsEvent>(_employeePositionsViewModel, "Должности",null),
					//    new NavigationItem<ShowEmployeeDepartmentsEvent>(_employeeDepartmentsViewModel, "Подразделения",null),
					//    new NavigationItem<ShowEmployeeGroupsEvent>(_employeeGroupsViewModel, "Группы",null),
					//}),
                    new NavigationItem<ShowSKDDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowSKDZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowSKDLidraryEvent, object>(LibraryViewModel, "Библиотека", "/Controls;component/Images/book.png"),
					new NavigationItem<ShowSKDSettingsEvent, object>(SettingsViewModel, "Настройки", "/Controls;component/Images/book.png"),
					new NavigationItem<ShowSKDFiltersEvent, Guid>(FiltersViewModel, "Фильтры", "/Controls;component/Images/book.png", null, null, Guid.Empty),
					new NavigationItem("Интервалы",null, new List<NavigationItem>()
					{
						new NavigationItem<ShowSKDTimeIntervalsEvent, Guid>(TimeIntervalsViewModel, "Именованные интервалы", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
						new NavigationItem<ShowSKDSlideDayIntervalsEvent, Guid>(SlideDayIntervalsViewModel, "Скользящие посуточные графики", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
						new NavigationItem<ShowSKDSlideWeekIntervalsEvent, Guid>(SlideWeekIntervalsViewModel, "Скользящие понедельные графики", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
						new NavigationItem<ShowSKDWeeklyIntervalsEvent, Guid>(WeeklyIntervalsViewModel, "Недельные графики", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
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
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Shedule/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Settings/DataTemplates/Dictionary.xaml"));
		}

		private void OnCreateSKDZone(CreateSKDZoneEventArg createZoneEventArg)
		{
			ZonesViewModel.CreateZone(createZoneEventArg);
		}
		private void OnEditSKDZone(Guid zoneUID)
		{
			ZonesViewModel.EditZone(zoneUID);
		}
	}
}