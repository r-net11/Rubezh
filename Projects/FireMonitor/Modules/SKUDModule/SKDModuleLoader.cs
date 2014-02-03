using System.Collections.Generic;
using System.Linq;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using SKDModule.ViewModels;
using FiresecClient;
using FiresecAPI;
using System;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Services;
using Infrastructure;

namespace SKDModule
{
	public class SKDModuleLoader : ModuleBase
	{
		SKUDViewModel SKUDViewModel;
		NavigationItem _skudNavigationItem;
		JournalViewModel JournalViewModel;
		DevicesViewModel DevicesViewModel;
		ZonesViewModel ZonesViewModel;
		VerificationViewModel VerificationViewModel;
		UsersAccessViewModel UsersAccessViewModel;

		public override void CreateViewModels()
		{
			SKUDViewModel = new SKUDViewModel();
			JournalViewModel = new JournalViewModel();
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			VerificationViewModel = new VerificationViewModel();
			UsersAccessViewModel = new UsersAccessViewModel();
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_skudNavigationItem = new NavigationItem<ShowSKUDEvent>(SKUDViewModel, "СКД", "/Controls;component/Images/levels.png");
			return new List<NavigationItem>
				{
				new NavigationItem("СКД", "/Controls;component/Images/tree.png",
					new List<NavigationItem>()
					{
						_skudNavigationItem,
						new NavigationItem<ShowSKDJournalEvent>(JournalViewModel, "Журнал", "/Controls;component/Images/levels.png"),
						new NavigationItem<ShowSKDDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
						new NavigationItem<ShowSKDZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
						new NavigationItem<ShowSKDVerificationEvent>(VerificationViewModel, "Верификация", "/Controls;component/Images/tree.png"),
						new NavigationItem<ShowSKDUsersAccessEvent>(UsersAccessViewModel, "Доступ пользователей", "/Controls;component/Images/tree.png"),
					})
				};
		}

		public override void Initialize()
		{
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
		}

		public override string Name
		{
			get { return "СКД"; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Journal/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Devices/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Zones/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Verification/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Access/DataTemplates/Dictionary.xaml"));
		}

		public override bool BeforeInitialize(bool firstTime)
		{
			SKDManager.CreateStates();
			return true;
		}

		public override void AfterInitialize()
		{
			SafeFiresecService.SKDCallbackResultEvent -= new Action<SKDCallbackResult>(OnSKDCallbackResult);
			SafeFiresecService.SKDCallbackResultEvent += new Action<SKDCallbackResult>(OnSKDCallbackResult);
			ServiceFactoryBase.Events.GetEvent<SKDObjectsStateChangedEvent>().Publish(null);
			AutoActivationWatcher.Run();
		}

		void OnSKDCallbackResult(SKDCallbackResult skdCallbackResult)
		{
			ApplicationService.Invoke(() =>
			{
				if (skdCallbackResult.JournalItems.Count > 0)
				{
					ServiceFactory.Events.GetEvent<NewSKDJournalEvent>().Publish(skdCallbackResult.JournalItems);
				}
				CopySKDStates(skdCallbackResult.SKDStates);
				ServiceFactoryBase.Events.GetEvent<SKDObjectsStateChangedEvent>().Publish(null);
			});
		}

		void CopySKDStates(SKDStates skdStates)
		{
			foreach (var remoteDeviceState in skdStates.DeviceStates)
			{
				var device = SKDManager.Devices.FirstOrDefault(x => x.UID == remoteDeviceState.UID);
				if (device != null)
				{
					remoteDeviceState.CopyTo(device.State);
					device.State.OnStateChanged();
				}
			}
		}
	}
}