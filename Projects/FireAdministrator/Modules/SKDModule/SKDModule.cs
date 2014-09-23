using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using FiresecAPI.SKD;
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
using SKDModule.Plans;
using SKDModule.Validation;
using SKDModule.ViewModels;
using FiresecAPI;
using Infrastructure.Common.Windows;

namespace SKDModule
{
	public class SKDModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
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
				new NavigationItem("СКД", "/Controls;component/Images/SKDW.png", new List<NavigationItem>()
				{
					new NavigationItem<ShowSKDDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/Tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowSKDZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/Zones.png", null, null, Guid.Empty),
					new NavigationItem<ShowSKDDoorEvent, Guid>(DoorsViewModel, "Точки доступа", "/Controls;component/Images/DoorW.png", null, null, Guid.Empty),
					new NavigationItem("Графики", "/Controls;component/Images/ShedulesW.png", new List<NavigationItem>()
					{
						new NavigationItem<ShowSKDDayIntervalsEvent, int>(DayIntervalsViewModel, "Дневные графики", "/Controls;component/Images/ShedulesDaylyW.png", null, null, -1),
						new NavigationItem<ShowSKDWeeklyIntervalsEvent, int>(WeeklyIntervalsViewModel, "Недельные графики", "/Controls;component/Images/SheduleWeeklyW.png", null, null, -1),
						//new NavigationItem<ShowSKDSlideDayIntervalsEvent, int>(SlideDayIntervalsViewModel, "Скользящие посуточные графики", "/Controls;component/Images/SheduleSlideDaylyW.png", null, null, -1),
						//new NavigationItem<ShowSKDSlideWeekIntervalsEvent, int>(SlideWeekIntervalsViewModel, "Скользящие понедельные графики", "/Controls;component/Images/SheduleSlideWeeklyW.png", null, null, -1),
						//new NavigationItem<ShowSKDHolidaysEvent, Guid>(HolidaysViewModel, "Праздничные дни", "/Controls;component/Images/HolidaysW.png", null, null, Guid.Empty),
					}),
					#if DEBUG
					new NavigationItem<ShowSKDLidraryEvent, object>(LibraryViewModel, "Библиотека", "/Controls;component/Images/Book.png"),
					#endif
				}) {IsExpanded = true},
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
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDHR, 304, "Картотека", "Панель картотека", "BLevels.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDVerification, 305, "Верификация", "Панель верификация", "BTree.png") { Factory = (p) => new LayoutPartVerificationViewModel(p as LayoutPartReferenceProperties), };
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDDayIntervals, 306, "Дневные графики", "Панель дневные графики", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDScheduleSchemes, 307, "Графики", "Панель графики", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDHolidays, 308, "Праздничные дни", "Панель праздничные дни", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDSchedules, 309, "Графики работ", "Панель графики работ", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDTimeTracking, 310, "УРВ", "Панель учета рабочеговремени", "BTree.png");
		}
		#endregion
	}
}