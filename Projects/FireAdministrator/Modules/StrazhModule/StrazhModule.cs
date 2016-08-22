using Localization.Strazh.Common;
using FiresecClient;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Layouts;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Events;
using StrazhAPI;
using StrazhAPI.Enums;
using StrazhAPI.Models;
using StrazhAPI.SKD.Device;
using StrazhModule.Events;
using StrazhModule.Plans;
using StrazhModule.Validation;
using StrazhModule.ViewModels;
using System;
using System.Collections.Generic;

namespace StrazhModule
{
	public class StrazhModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
	{
		private DevicesViewModel _devicesViewModel;
		private ZonesViewModel _zonesViewModel;
		private DoorsViewModel _doorsViewModel;
		private DayIntervalsViewModel _dayIntervalsViewModel;
		private WeeklyIntervalsViewModel _weeklyIntervalsViewModel;
		private SlideDayIntervalsViewModel _slideDayIntervalsViewModel;
		private SlideWeekIntervalsViewModel _slideWeekIntervalsViewModel;
		private HolidaysViewModel _holidaysViewModel;
		private LibraryViewModel _libraryViewModel;
		private SKDPlanExtension _planExtension;
		private DoorDayIntervalsViewModel _doorDayIntervalsViewModel;
		private DoorWeeklyIntervalsViewModel _doorWeeklyIntervalsViewModel;

		public override void CreateViewModels()
		{
			ServiceFactoryBase.Events.GetEvent<CreateSKDZoneEvent>().Subscribe(OnCreateSKDZone);
			ServiceFactoryBase.Events.GetEvent<EditSKDZoneEvent>().Subscribe(OnEditSKDZone);
			ServiceFactoryBase.Events.GetEvent<CreateDoorEvent>().Subscribe(OnCreateDoor);
			ServiceFactoryBase.Events.GetEvent<EditDoorEvent>().Subscribe(OnEditDoor);

			_devicesViewModel = new DevicesViewModel();
			_zonesViewModel = new ZonesViewModel();
			_doorsViewModel = new DoorsViewModel();
			_dayIntervalsViewModel = new DayIntervalsViewModel();
			_weeklyIntervalsViewModel = new WeeklyIntervalsViewModel();
			_slideDayIntervalsViewModel = new SlideDayIntervalsViewModel();
			_slideWeekIntervalsViewModel = new SlideWeekIntervalsViewModel();
			_holidaysViewModel = new HolidaysViewModel();
			_libraryViewModel = new LibraryViewModel();
			_planExtension = new SKDPlanExtension(_devicesViewModel, _zonesViewModel, _doorsViewModel);
			_doorDayIntervalsViewModel = new DoorDayIntervalsViewModel();
			_doorWeeklyIntervalsViewModel = new DoorWeeklyIntervalsViewModel();
		}

		public override void Initialize()
		{
			_devicesViewModel.Initialize();
			_zonesViewModel.Initialize();
			_doorsViewModel.Initialize();

			_dayIntervalsViewModel.Initialize();
			_weeklyIntervalsViewModel.Initialize();
			_slideDayIntervalsViewModel.Initialize();
			_slideWeekIntervalsViewModel.Initialize();
			_holidaysViewModel.Initialize();

			_planExtension.Initialize();
			ServiceFactoryBase.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
			_planExtension.Cache.BuildAllSafe();
			_doorDayIntervalsViewModel.Initialize();
			_doorWeeklyIntervalsViewModel.Initialize();
		}

		public override void AfterInitialize()
		{
			base.AfterInitialize();
			SafeFiresecService.NewSearchDeviceEvent -= OnNewSearchDeviceEvent;
			SafeFiresecService.NewSearchDeviceEvent += OnNewSearchDeviceEvent;
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>
			{
				new NavigationItem(ModuleType.ToDescription(), "SKDW", new List<NavigationItem>
				{
					new NavigationItem<ShowSKDDeviceEvent, Guid>(_devicesViewModel, CommonResources.Devices, "Tree", null, null, Guid.Empty),
					new NavigationItemEx<ShowSKDZoneEvent, Guid>(_zonesViewModel, CommonResources.Zones, "Zones", null, null, Guid.Empty),
					new NavigationItemEx<ShowSKDDoorEvent, Guid>(_doorsViewModel, CommonResources.Doors, "DoorW", null, null, Guid.Empty),
					new NavigationItem(CommonResources.AccessSchedules, "ShedulesW", new List<NavigationItem>
					{
						new NavigationItem<ShowSKDDayIntervalsEvent, Guid>(_dayIntervalsViewModel, CommonResources.DayAccessSchedules, "ShedulesDaylyW", null, null, Guid.Empty),
						new NavigationItem<ShowSKDWeeklyIntervalsEvent, int>(_weeklyIntervalsViewModel, CommonResources.WeekAccessSchedules, "SheduleWeeklyW", null, null, -1),
					}),
					new NavigationItem(CommonResources.LockSchedules, "ShedulesW", new List<NavigationItem>
					{
						new NavigationItem<ShowSKDDoorDayIntervalsEvent, Guid>(_doorDayIntervalsViewModel, CommonResources.DayLockSchedules, "ShedulesDaylyW", null, null, Guid.Empty),
						new NavigationItem<ShowSKDDoorWeeklyIntervalsEvent, int>(_doorWeeklyIntervalsViewModel, CommonResources.WeekLockSchedules, "SheduleWeeklyW", null, null, -1),
					}),
#if DEBUG
					new NavigationItem<ShowSKDLidraryEvent, object>(_libraryViewModel, CommonResources.Library, "Book"),
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
			_zonesViewModel.CreateZone(createZoneEventArg);
		}
		private void OnEditSKDZone(Guid zoneUID)
		{
			_zonesViewModel.EditZone(zoneUID);
		}
		private void OnCreateDoor(CreateDoorEventArg createDoorEventArg)
		{
			_doorsViewModel.CreateDoor(createDoorEventArg);
		}
		private void OnEditDoor(Guid doorUID)
		{
			_doorsViewModel.EditDoor(doorUID);
		}

		private static void OnNewSearchDeviceEvent(SKDDeviceSearchInfo skdDeviceSearchInfo)
		{
			ApplicationService.Invoke(() => ServiceFactoryBase.Events.GetEvent<SKDSearchDeviceEvent>().Publish(new List<SKDDeviceSearchInfo> {skdDeviceSearchInfo}));
		}

		#region ILayoutDeclarationModule Members
		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDDevices, 301, CommonResources.SKDDevices, CommonResources.SKDDevicePanel, "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDZones, 302, CommonResources.SKDZones, CommonResources.SKDZonePanel, "BTree.png");
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.SKD, LayoutPartIdentities.SKDDoors, 303, CommonResources.Doors, CommonResources.DoorPanel, "BTree.png");
		}
		#endregion
	}
}