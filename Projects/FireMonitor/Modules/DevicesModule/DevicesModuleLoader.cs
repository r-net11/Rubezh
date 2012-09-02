using System;
using System.Collections.Generic;
using DevicesModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using FiresecClient;
using Infrastructure.Common.Reports;
using DevicesModule.Reports;
using Infrastructure.Client;

namespace DevicesModule
{
	public class DevicesModuleLoader : ModuleBase, IReportProviderModule
	{
		static DevicesViewModel DevicesViewModel;
		static ZonesViewModel ZonesViewModel;
		private NavigationItem _zonesNavigationItem;

		public DevicesModuleLoader()
		{
			ServiceFactory.Layout.AddToolbarItem(new ConnectionIndicatorViewModel());
			ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Subscribe(OnShowDeviceDetails);
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
		}

		void OnShowDeviceDetails(Guid deviceUID)
		{
			DialogService.ShowWindow(new DeviceDetailsViewModel(deviceUID));
		}

		public override void Initialize()
		{
			_zonesNavigationItem.IsVisible = FiresecManager.FiresecConfiguration.DeviceConfiguration.Zones.Count > 0;
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_zonesNavigationItem = new NavigationItem<ShowZoneEvent, int?>(ZonesViewModel, "Зоны", "/Controls;component/Images/zones.png");
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
				_zonesNavigationItem
			};
		}

		public override string Name
		{
			get { return "Устройства и Зоны"; }
		}

		#region IReportProviderModule Members

		public IEnumerable<IReportProvider> GetReportProviders()
		{
			return new List<IReportProvider>()
			{
				new DeviceParamsReport(),
				new DeviceListReport(),
				new DriverCounterReport(),
				new IndicationBlockReport(),
			};
		}

		#endregion
	}
}