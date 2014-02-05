using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using SKDModule.Events;
using SKDModule.ViewModels;
using System;
using System.Linq;
using XFiresecAPI;
using FiresecAPI;
using SKDModule.Plans;
using Infrustructure.Plans.Events;
using SKDModule.Plans.Designer;
using Infrastructure.Common.Validation;
using SKDModule.Validation;

namespace SKDModule
{
	public class SKDModule : ModuleBase, IValidationModule
	{
		DevicesViewModel DevicesViewModel;
		ZonesViewModel ZonesViewModel;
		LibraryViewModel LibraryViewModel;
		TimeIntervalsViewModel TimeIntervalsViewModel;
		SlideDayIntervalsViewModel SlideDayIntervalsViewModel;
		SlideWeekIntervalsViewModel SlideWeekIntervalsViewModel;
		WeeklyIntervalsViewModel WeeklyIntervalsViewModel;
		HolidaysViewModel HolidaysViewModel;
		FiltersViewModel FiltersViewModel;
		PassCardsDesignerViewModel PassCardDesignerViewModel;
		SettingsViewModel SettingsViewModel;
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
			SlideDayIntervalsViewModel = new SlideDayIntervalsViewModel();
			SlideWeekIntervalsViewModel = new SlideWeekIntervalsViewModel();
			WeeklyIntervalsViewModel = new WeeklyIntervalsViewModel();
			HolidaysViewModel = new HolidaysViewModel();
			SettingsViewModel = new SettingsViewModel();
			FiltersViewModel = new FiltersViewModel();
			_planExtension = new SKDPlanExtension(DevicesViewModel, ZonesViewModel);
		}

		public override void Initialize()
		{
			PassCardDesignerViewModel.Initialize();
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
				new NavigationItem("СКД", null, new List<NavigationItem>()
				{
                    new NavigationItem<ShowSKDDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowSKDZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowSKDLidraryEvent, object>(LibraryViewModel, "Библиотека", "/Controls;component/Images/book.png"),
					new NavigationItem<ShowSKDFiltersEvent, Guid>(FiltersViewModel, "Фильтры", "/Controls;component/Images/book.png", null, null, Guid.Empty),
					new NavigationItem<ShowPassCardDesignerEvent>(PassCardDesignerViewModel, "Дизайнер пропусков",null),
					new NavigationItem<ShowSKDSettingsEvent, object>(SettingsViewModel, "Настройки", "/Controls;component/Images/book.png"),
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
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Devices/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Zones/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Library/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Filters/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "PassCard/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Shedule/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Settings/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
		}

		#region IValidationModule Members
		public IEnumerable<IValidationError> Validate()
		{
			return Validator.Validate();
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
	}
}