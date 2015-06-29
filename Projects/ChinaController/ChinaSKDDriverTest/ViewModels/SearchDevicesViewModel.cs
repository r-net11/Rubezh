using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ChinaSKDDriver;

namespace ControllerSDK.ViewModels
{
	public class SearchDevicesViewModel : BaseViewModel
	{
		public SearchDevicesViewModel()
		{
			SearchDevices = new ObservableCollection<SearchDeviceViewModel>();

			SearchDevicesCommand = new RelayCommand(OnSearchDevices);
			AddDeviceCommand = new RelayCommand(OnAddDevice, CanAddDevice);
	
			Wrapper.NewSearchDevice += WrapperOnNewSearchDevice;
		}

		public ObservableCollection<SearchDeviceViewModel> SearchDevices { get; private set; }

		public SearchDeviceViewModel SelectedSearchDevice { get; set; }

		private void WrapperOnNewSearchDevice(SearchDevicesEventArgs e)
		{
			if (SearchDevices.Any((x) => x.Mac == e.DeviceSearchInfo.Mac))
				return;

			var deviceSearchInfo = e.DeviceSearchInfo;
			ApplicationService.BeginInvoke(new Action(() => SearchDevices.Add(new SearchDeviceViewModel
			{
				IpAddress = deviceSearchInfo.IpAddress,
				Port = deviceSearchInfo.Port,
				Submask = deviceSearchInfo.Submask,
				Gateway = deviceSearchInfo.Gateway,
				Mac = deviceSearchInfo.Mac
			})));
		}

		public RelayCommand SearchDevicesCommand { get; private set; }
		private void OnSearchDevices()
		{
			ApplicationService.BeginInvoke(new Action(() =>
			{
				SearchDevices.Clear();
			}));
			MainViewModel.Wrapper.StartSearchDevices();
		}

		public RelayCommand AddDeviceCommand { get; private set; }
		private void OnAddDevice()
		{
			MessageBox.Show(String.Format("{0}:{1}", SelectedSearchDevice.IpAddress, SelectedSearchDevice.Port));
		}
		private bool CanAddDevice()
		{
			return SelectedSearchDevice != null;
		}
	}
}
