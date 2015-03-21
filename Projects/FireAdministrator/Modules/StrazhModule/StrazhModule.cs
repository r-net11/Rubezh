using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using Infrustructure.Plans.Events;
using StrazhModule.Events;
using StrazhModule.Plans;
using StrazhModule.Validation;
using StrazhModule.ViewModels;
using FiresecAPI.SKD;

namespace StrazhModule
{
	public class StrazhModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
	{
		DevicesViewModel DevicesViewModel;
		ZonesViewModel ZonesViewModel;
		DoorsViewModel DoorsViewModel;
		DayIntervalsViewModel DayIntervalsViewModel;
		WeeklyIntervalsViewModel WeeklyIntervalsViewModel;
		SlideDayIntervalsViewModel SlideDayIntervalsViewModel;
		SlideWeekIntervalsViewModel SlideWeekIntervalsViewModel;
		HolidaysViewModel HolidaysViewModel;
		LibraryViewModel LibraryViewModel;
		SKDPlanExtension _planExtension;

		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<CreateSKDZoneEvent>().Subscribe(OnCreateSKDZone);
			ServiceFactory.Events.GetEvent<EditSKDZoneEvent>().Subscribe(OnEditSKDZone);
			ServiceFactory.Events.GetEvent<CreateDoorEvent>().Subscribe(OnCreateDoor);
			ServiceFactory.Events.GetEvent<EditDoorEvent>().Subscribe(OnEditDoor);

			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			DoorsViewModel = new DoorsViewModel();
			DayIntervalsViewModel = new DayIntervalsViewModel();
			WeeklyIntervalsViewModel = new WeeklyIntervalsViewModel();
			SlideDayIntervalsViewModel = new SlideDayIntervalsViewModel();
			SlideWeekIntervalsViewModel = new SlideWeekIntervalsViewModel();
			HolidaysViewModel = new HolidaysViewModel();
			LibraryViewModel = new LibraryViewModel();
			_planExtension = new SKDPlanExtension(DevicesViewModel, ZonesViewModel, DoorsViewModel);
		}

		public override void Initialize()
		{
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
			DoorsViewModel.Initialize();

			DayIntervalsViewModel.Initialize();
			WeeklyIntervalsViewModel.Initialize();
			SlideDayIntervalsViewModel.Initialize();
			SlideWeekIntervalsViewModel.Initialize();
			HolidaysViewModel.Initialize();

			_planExtension.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
			_planExtension.Cache.BuildAllSafe();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem(ModuleType.ToDescription(), "SKDW", new List<NavigationItem>()
				{
					new NavigationItem<ShowSKDDeviceEvent, Guid>(DevicesViewModel, "Устройства", "Tree", null, null, Guid.Empty),
					new NavigationItemEx<ShowSKDZoneEvent, Guid>(ZonesViewModel, "Зоны", "Zones", null, null, Guid.Empty),
					new NavigationItemEx<ShowSKDDoorEvent, Guid>(DoorsViewModel, "Точки доступа", "DoorW", null, null, Guid.Empty),
					new NavigationItem("Графики", "ShedulesW", new List<NavigationItem>()
					{
						new NavigationItem<ShowSKDDayIntervalsEvent, Guid>(DayIntervalsViewModel, "Дневные графики", "ShedulesDaylyW", null, null, Guid.Empty),
						new NavigationItem<ShowSKDWeeklyIntervalsEvent, int>(WeeklyIntervalsViewModel, "Недельные графики", "SheduleWeeklyW", null, null, -1),
						//new NavigationItem<ShowSKDSlideDayIntervalsEvent, int>(SlideDayIntervalsViewModel, "Скользящие посуточные графики", "SheduleSlideDaylyW", null, null, -1),
						//new NavigationItem<ShowSKDSlideWeekIntervalsEvent, int>(SlideWeekIntervalsViewModel, "Скользящие понедельные графики", "SheduleSlideWeeklyW", null, null, -1),
						//new NavigationItem<ShowSKDHolidaysEvent, Guid>(HolidaysViewModel, "Праздничные дни", "HolidaysW", null, null, Guid.Empty),
					}),
					#if DEBUG
					new NavigationItem<ShowSKDLidraryEvent, object>(LibraryViewModel, "Библиотека", "Book"),
					#endif
				}) {IsExpanded = true},
			};
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Strazh; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Devices/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Zones/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Doors/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Library/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "TimeIntervals/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Layout/DataTemplates/Dictionary.xaml"));
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
		private void OnCreateDoor(CreateDoorEventArg createDoorEventArg)
		{
			DoorsViewModel.CreateDoor(createDoorEventArg);
		}
		private void OnEditDoor(Guid doorUID)
		{
			DoorsViewModel.EditDoor(doorUID);
		}

		#region ILayoutDeclarationModule Members
		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDDevices, 301, "СКД устройства", "Панель СКД устройства", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDZones, 302, "СКД зоны", "Панель СКД зоны", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDDoors, 303, "Точки доступа", "Панель точек досткпа", "BMPT.png");
		}
		#endregion
	}
}