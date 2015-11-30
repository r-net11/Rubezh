using System.Linq;
using Infrastructure.Common.SKDReports;
using RubezhAPI.SKD.ReportFilters;
using RubezhClient;
using RubezhAPI.GK;
using System.Collections.Generic;

namespace GKModule.ViewModels
{
	class DevicesPageViewModel : FilterContainerViewModel
	{
		public List<GKDevice> SelectedDevices { get; private set; }
		public ReportFilterDeviceViewModel[] Devices { get; private set; }
		public DevicesPageViewModel()
		{
			Title = "Устройства";
			Initialize();
		}
		ReportFilterDeviceViewModel _rootDevice;
		public ReportFilterDeviceViewModel RootDevice
		{
			get { return _rootDevice; }
			private set
			{
				_rootDevice = value;
				OnPropertyChanged(() => RootDevice);
			}
		}
		public void Initialize()
		{
			BuildTree();
			if (RootDevice != null)
			{
				RootDevice.IsExpanded = true;
			}

			foreach (var device in AllDevices)
			{
				if (device.Device.DriverType == GKDriverType.RSR2_KAU)
					device.ExpandToThis();
			}

			Devices = null;
			OnPropertyChanged(() => Devices);
			Devices = new ReportFilterDeviceViewModel[] { RootDevice };
			OnPropertyChanged(() => Devices);
		}
		void BuildTree()
		{
			RootDevice = AddDeviceInternal(GKManager.DeviceConfiguration.RootDevice, null);
			FillAllDevices();
		}
		ReportFilterDeviceViewModel AddDeviceInternal(GKDevice device, ReportFilterDeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new ReportFilterDeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				if (!childDevice.IsNotUsed)
				{
					AddDeviceInternal(childDevice, deviceViewModel);
				}
			}
			return deviceViewModel;
		}
		public List<ReportFilterDeviceViewModel> AllDevices;
		public void FillAllDevices()
		{
			AllDevices = new List<ReportFilterDeviceViewModel>();
			AddChildPlainDevices(RootDevice);
		}
		void AddChildPlainDevices(ReportFilterDeviceViewModel parentViewModel)
		{
			AllDevices.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainDevices(childViewModel);
		}
		public override void LoadFilter(SKDReportFilter filter)
		{
			var devicesFilter = filter as DevicesReportFilter;
			if (devicesFilter == null)
				return;
			if (!filter.IsDefault)
			{
				foreach (var gkDevice in devicesFilter.SelectedDevices)
				{
					if (AllDevices.FirstOrDefault(x => x.Device == gkDevice) != null)
						AllDevices.FirstOrDefault(x => x.Device == gkDevice).IsChecked = true;
				}
			}
			else
			{
				AllDevices.ForEach(x => x.IsChecked = true);
			}
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var devicesFilter = filter as DevicesReportFilter;
			if (devicesFilter == null)
				return;
			devicesFilter.SelectedDevices.Clear();
			foreach (var selectedDevice in AllDevices.Where(x => x.IsChecked == true))
			{
				devicesFilter.SelectedDevices.Add(selectedDevice.Device);
			}
		}
	}
}