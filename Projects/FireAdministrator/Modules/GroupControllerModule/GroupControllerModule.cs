using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using GKModule.Plans;
using GKModule.Plans.Designer;
using GKModule.Validation;
using GKModule.ViewModels;
using GKProcessor;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using XFiresecAPI;

namespace GKModule
{
	public class GroupControllerModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
	{
		DevicesViewModel DevicesViewModel;
		ParameterTemplatesViewModel ParameterTemplatesViewModel;
		ZonesViewModel ZonesViewModel;
		DirectionsViewModel DirectionsViewModel;
		PumpStationsViewModel PumpStationsViewModel;
		FiltersViewModel FiltersViewModel;
		MPTsViewModel MPTsViewModel;
		GuardViewModel GuardViewModel;
		LibraryViewModel DeviceLidraryViewModel;
		InstructionsViewModel InstructionsViewModel;
		OPCDevicesViewModel OPCDevicesViewModel;
		OPCZonesViewModel OPCZonesViewModel;
		OPCDirectionsViewModel OPCDirectionsViewModel;
		DiagnosticsViewModel DiagnosticsViewModel;
		DescriptorsViewModel DescriptorsViewModel;
		GKPlanExtension _planExtension;

		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<CreateXZoneEvent>().Subscribe(OnCreateXZone);
			ServiceFactory.Events.GetEvent<EditXZoneEvent>().Subscribe(OnEditXZone);
			ServiceFactory.Events.GetEvent<CreateXDirectionEvent>().Subscribe(OnCreateXDirection);
			ServiceFactory.Events.GetEvent<EditXDirectionEvent>().Subscribe(OnEditXDirection);

			DevicesViewModel = new DevicesViewModel();
			ParameterTemplatesViewModel = new ParameterTemplatesViewModel();
			ZonesViewModel = new ZonesViewModel();
			DirectionsViewModel = new DirectionsViewModel();
			PumpStationsViewModel = new PumpStationsViewModel();
			FiltersViewModel = new FiltersViewModel();
			GuardViewModel = new GuardViewModel();
			MPTsViewModel = new MPTsViewModel();
			DeviceLidraryViewModel = new LibraryViewModel();
			InstructionsViewModel = new InstructionsViewModel();
			OPCDevicesViewModel = new OPCDevicesViewModel();
			OPCZonesViewModel = new OPCZonesViewModel();
			OPCDirectionsViewModel = new OPCDirectionsViewModel();
			DiagnosticsViewModel = new DiagnosticsViewModel();
			DescriptorsViewModel = new DescriptorsViewModel();
			_planExtension = new GKPlanExtension(DevicesViewModel, ZonesViewModel, DirectionsViewModel);
		}

		public override void Initialize()
		{
			DevicesViewModel.Initialize();
			ParameterTemplatesViewModel.Initialize();
			ZonesViewModel.Initialize();
			DirectionsViewModel.Initialize();
			PumpStationsViewModel.Initialize();
			MPTsViewModel.Initialize();
			GuardViewModel.Initialize();
			FiltersViewModel.Initialize();
			InstructionsViewModel.Initialize();
			OPCDevicesViewModel.Initialize();
			OPCZonesViewModel.Initialize();
			OPCDirectionsViewModel.Initialize();

			_planExtension.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
			Helper.BuildMap();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>
				{
					new NavigationItem<ShowXDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowXParameterTemplatesEvent, Guid>(ParameterTemplatesViewModel, "Шаблоны","/Controls;component/Images/briefcase.png", null, null, Guid.Empty),
					new NavigationItem<ShowXZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/zones.png", null, null, Guid.Empty),
					new NavigationItem<ShowXDirectionEvent, Guid>(DirectionsViewModel, "Направления", "/Controls;component/Images/direction.png", null, null, Guid.Empty),
					new NavigationItem<ShowXPumpStationEvent, Guid>(PumpStationsViewModel, "НС", "/Controls;component/Images/PumpStation.png", null, null, Guid.Empty),
					new NavigationItem<ShowXMPTEvent, Guid>(MPTsViewModel, "МПТ", "/Controls;component/Images/MPT.png", null, null, Guid.Empty),
					new NavigationItem<ShowXGuardEvent, Guid>(GuardViewModel, "Охрана", "/Controls;component/Images/user.png", null, null, Guid.Empty),
					new NavigationItem<ShowXJournalFilterEvent, object>(FiltersViewModel, "Фильтры", "/Controls;component/Images/filter.png"),
					new NavigationItem<ShowXDeviceLidraryEvent, object>(DeviceLidraryViewModel, "Библиотека", "/Controls;component/Images/book.png"),
					new NavigationItem<ShowXInstructionsEvent, Guid>(InstructionsViewModel, "Инструкции", "/Controls;component/Images/information.png", null, null, Guid.Empty),
					new NavigationItem("OPC Сервер", "/Controls;component/Images/tree.png",
						new List<NavigationItem>()
						{
							new NavigationItem<ShowXOPCDevicesEvent, Guid>(OPCDevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
							new NavigationItem<ShowXOPCZonesEvent, Guid>(OPCZonesViewModel, "Зоны", "/Controls;component/Images/zones.png", null, null, Guid.Empty),
							new NavigationItem<ShowXOPCDirectionsEvent, Guid>(OPCDirectionsViewModel, "Направления", "/Controls;component/Images/direction.png", null, null, Guid.Empty),
						}),
					new NavigationItem<ShowXDiagnosticsEvent, object>(DiagnosticsViewModel, "Диагностика", "/Controls;component/Images/Bug.png"),
#if DEBUG
					new NavigationItem<ShowXDescriptorsEvent, object>(DescriptorsViewModel, "Дескрипторы", "/Controls;component/Images/Descriptors.png"),
					#endif
				};
		}
		public override string Name
		{
			get { return "Групповой контроллер"; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Parameters/DataTemplates/Dictionary.xaml"));
		}

		#region IValidationModule Members
		public IEnumerable<IValidationError> Validate()
		{
			var validator = new Validator();
			return validator.Validate();
		}
		#endregion

		private void OnCreateXZone(CreateXZoneEventArg createZoneEventArg)
		{
			ZonesViewModel.CreateZone(createZoneEventArg);
		}
		private void OnEditXZone(Guid zoneUID)
		{
			ZonesViewModel.EditZone(zoneUID);
		}

		private void OnCreateXDirection(CreateXDirectionEventArg createDirectionEventArg)
		{
			DirectionsViewModel.CreateDirection(createDirectionEventArg);
		}
		private void OnEditXDirection(Guid directionUID)
		{
			DirectionsViewModel.EditDirection(directionUID);
		}

		public override bool BeforeInitialize(bool firstTime)
		{
			LoadingService.DoStep("Загрузка конфигурации ГК");
			GKDriversCreator.Create();
			XManager.UpdateConfiguration();

			GKProcessorManager.NewJournalItem -= new Action<JournalItem, bool>(OnNewJournalItems);
			GKProcessorManager.NewJournalItem += new Action<JournalItem, bool>(OnNewJournalItems);

			SafeFiresecService.GKProgressCallbackEvent -= new Action<FiresecAPI.GKProgressCallback>(OnGKProgressCallbackEvent);
			SafeFiresecService.GKProgressCallbackEvent += new Action<FiresecAPI.GKProgressCallback>(OnGKProgressCallbackEvent);

			GKProcessorManager.GKProgressCallbackEvent -= new Action<FiresecAPI.GKProgressCallback>(OnGKProgressCallbackEvent);
			GKProcessorManager.GKProgressCallbackEvent += new Action<FiresecAPI.GKProgressCallback>(OnGKProgressCallbackEvent);
			return true;
		}

		public override void AfterInitialize()
		{
			FiresecManager.StartPoll(true);
		}

		void OnNewJournalItems(JournalItem journalItem, bool isAdministrator)
		{
			if (isAdministrator)
			{
				FiresecManager.FiresecService.AddJournalItem(journalItem);
			}
		}

		void OnGKProgressCallbackEvent(GKProgressCallback gkProgressCallback)
		{
			ApplicationService.Invoke(() =>
			{
				switch (gkProgressCallback.GKProgressCallbackType)
				{
					case GKProgressCallbackType.Start:
						if (gkProgressCallback.GKProgressClientType == GKProgressClientType.Administrator)
						{
							LoadingService.Show(gkProgressCallback.Title, gkProgressCallback.Text, gkProgressCallback.StepCount, gkProgressCallback.CanCancel);
						}
						return;

					case GKProgressCallbackType.Progress:
						if (gkProgressCallback.GKProgressClientType == GKProgressClientType.Administrator)
						{
							LoadingService.DoStep(gkProgressCallback.Text, gkProgressCallback.Title, gkProgressCallback.StepCount, gkProgressCallback.CanCancel);
							if (LoadingService.IsCanceled)
								FiresecManager.FiresecService.CancelGKProgress(gkProgressCallback.UID, FiresecManager.CurrentUser.Name);
						}
						return;

					case GKProgressCallbackType.Stop:
						LoadingService.Close();
						return;
				}
			});
		}

		#region ILayoutDeclarationModule Members

		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartIdentities.Alarms, 110, "Состояния", "Панель состояний", "BAlarm.png");
			yield return new LayoutPartDescription(LayoutPartIdentities.GDevices, 111, "Устройства", "Панель с устройствами", "BTree.png");
			yield return new LayoutPartDescription(LayoutPartIdentities.Zones, 112, "Зоны", "Панель зон", "BZones.png");
			yield return new LayoutPartDescription(LayoutPartIdentities.Directions, 113, "Направления", "Панель направления", "BDirection.png");
			yield return new LayoutPartDescription(LayoutPartIdentities.PumpStations, 114, "НС", "Панель НС", "BPumpStation.png");
			yield return new LayoutPartDescription(LayoutPartIdentities.MPTs, 115, "МПТ", "Панель МПТ", "BMPT.png");
			yield return new LayoutPartDescription(LayoutPartIdentities.Journals, 116, "Журнал событий", "Панель журнал событий", "BBook.png");
			yield return new LayoutPartDescription(LayoutPartIdentities.Archive, 117, "Архив", "Панель архив", "BArchive.png");
		}

		#endregion
	}
}