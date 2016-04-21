using System;
using System.Collections.Generic;
using Common;
using DevicesModule.Plans;
using DevicesModule.Plans.Designer;
using DevicesModule.Validation;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Validation;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Plans.Events;

namespace DevicesModule
{
	public class DevicesModule : ModuleBase, IValidationModule
	{
		NavigationItem _guardNavigationItem;
		DevicesViewModel DevicesViewModel;
		DeviceParametersViewModel DeviceParametersViewModel;
		ParameterTemplatesViewModel ParameterTemplatesViewModel;
		ZonesViewModel ZonesViewModel;
		DirectionsViewModel DirectionsViewModel;
		GuardViewModel GuardViewModel;
		SimulationViewModel SimulationViewModel;
		PlanExtension _planExtension;

		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<CreateZoneEvent>().Subscribe(OnCreateZone);
			ServiceFactory.Events.GetEvent<EditZoneEvent>().Subscribe(OnEditZone);
			ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Subscribe(OnShowDevice);

			DevicesViewModel = new DevicesViewModel();
			DeviceParametersViewModel = new DeviceParametersViewModel();
			ParameterTemplatesViewModel = new ParameterTemplatesViewModel();
			ZonesViewModel = new ZonesViewModel();
			DirectionsViewModel = new DirectionsViewModel();
			GuardViewModel = new GuardViewModel();
			SimulationViewModel = new SimulationViewModel();
			_planExtension = new PlanExtension(DevicesViewModel, ZonesViewModel);
		}

		void OnCreateZone(CreateZoneEventArg createZoneEventArg)
		{
			ZonesViewModel.CreateZone(createZoneEventArg);
		}
		void OnEditZone(Guid zoneUID)
		{
			ZonesViewModel.EditZone(zoneUID);
		}
		void OnShowDevice(Guid deviceUID)
		{
			DevicesViewModel.Select(deviceUID);
		}

		public override void RegisterResource()
		{
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Devices/DataTemplates/Dictionary.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Parameters/DataTemplates/Dictionary.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Zones/DataTemplates/Dictionary.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Directions/DataTemplates/Dictionary.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Guard/DataTemplates/Dictionary.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Simulation/DataTemplates/Dictionary.xaml"));
		}
		public override void Initialize()
		{
			DevicesViewModel.Initialize();
			DeviceParametersViewModel.Initialize();
			ParameterTemplatesViewModel.Initialize();
			ZonesViewModel.Initialize();
			DirectionsViewModel.Initialize();
			SimulationViewModel.Initialize();
			GuardViewModel.Initialize();
			_planExtension.Initialize();

			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
			Helper.BuildMap();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_guardNavigationItem = new NavigationItem<ShowGuardEvent>(GuardViewModel, "Охрана", "user") { IsVisible = false };
			ServiceFactory.Events.GetEvent<GuardVisibilityChangedEvent>().Subscribe(x => { _guardNavigationItem.IsVisible = x; });

			return new List<NavigationItem>()
			{
				new NavigationItem<ShowDeviceEvent, Guid>(DevicesViewModel, "Устройства","tree", null, null, Guid.Empty),
				new NavigationItem<ShowParameterTemplatesEvent, Guid>(ParameterTemplatesViewModel, "Шаблоны","briefcase", null, null, Guid.Empty),
				new NavigationItem<ShowZoneEvent, Guid>(ZonesViewModel, "Зоны","zones", null, null, Guid.Empty),
				new NavigationItem<ShowDirectionsEvent, Guid>(DirectionsViewModel, "Направления","direction", null, null, Guid.Empty) { IsVisible = false },
				new NavigationItem<ShowSimulationEvent, Guid>(SimulationViewModel, "Симуляция","Bug", null, null, Guid.Empty) { IsVisible = false },
				_guardNavigationItem
			};
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Devices; }
		}

		#region IValidationModule Members

		public IEnumerable<IValidationError> Validate()
		{
			var validator = new Validator(FiresecManager.FiresecConfiguration);
			return validator.Validate();
		}

		#endregion

		public override bool BeforeInitialize(bool firstTime)
		{
			try
			{
				//if (FiresecManager.IsFS2Enabled)
				//{
				//	LoadingService.DoStep("Инициализация драйвера устройств(FS2)");
				//	FiresecManager.InitializeFS2();
				//	LoadingService.DoStep("Старт мониторинга");
				//	FiresecManager.FS2ClientContract.Start();
				//}
				//else
				{
					LoadingService.DoStep("Загрузка драйвера устройств");
					var connectionResult = FiresecManager.InitializeFiresecDriver(false);
					if (connectionResult.HasError)
					{
						MessageBoxService.ShowError(connectionResult.Error);
						return false;
					}
					LoadingService.DoStep("Синхронизация конфигурации");
					FiresecManager.FiresecDriver.Synchronyze(false);
					LoadingService.DoStep("Старт мониторинга");
					FiresecManager.FiresecDriver.StartWatcher(false, false);
					FiresecManager.FSAgent.Start();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "DevicesModule.BeforeInitialize");
				MessageBoxService.ShowError(e.Message);
				return false;
			}
#if RELEASE
					if (LoadingErrorManager.HasError)
						MessageBoxService.ShowWarning(LoadingErrorManager.ToString(), "Ошибки при загрузке драйвера FireSec");
#endif
			return true;
		}
	}
}