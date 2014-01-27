using System.Collections.Generic;
using System.Linq;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using SKDModule.ViewModels;
using GKProcessor;
using FiresecClient;
using FiresecAPI.Models.Skud;
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
		JournalViewModel JournalViewModel;
		NavigationItem _skudNavigationItem;

		public override void CreateViewModels()
		{
			SKUDViewModel = new SKUDViewModel();
			JournalViewModel = new JournalViewModel();
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
						new NavigationItem<ShowSKDJournalEvent>(JournalViewModel, "Журнал", "/Controls;component/Images/levels.png")
					})
				};
		}

		public override void Initialize()
		{
			;
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
		}

		public override void AfterInitialize()
		{
			SafeFiresecService.SKDCallbackResultEvent -= new Action<SKDCallbackResult>(OnSKDCallbackResult);
			SafeFiresecService.SKDCallbackResultEvent += new Action<SKDCallbackResult>(OnSKDCallbackResult);

			ServiceFactoryBase.Events.GetEvent<SKDObjectsStateChangedEvent>().Publish(null);
		}

		void OnSKDCallbackResult(SKDCallbackResult gkCallbackResult)
		{
			ApplicationService.Invoke(() =>
			{
				if (gkCallbackResult.JournalItems.Count > 0)
				{
					ServiceFactory.Events.GetEvent<NewSKDJournalEvent>().Publish(gkCallbackResult.JournalItems);
				}
				CopyGKStates(gkCallbackResult.GKStates);
				ServiceFactoryBase.Events.GetEvent<SKDObjectsStateChangedEvent>().Publish(null);
			});
		}

		void CopyGKStates(SKDStates gkStates)
		{
			foreach (var remoteDeviceState in gkStates.DeviceStates)
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