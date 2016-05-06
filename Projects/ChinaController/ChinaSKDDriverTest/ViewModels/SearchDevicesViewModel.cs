using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using StrazhDeviceSDK;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using StrazhDeviceSDK.API;

namespace ControllerSDK.ViewModels
{
	public class SearchDevicesViewModel : BaseViewModel
	{
		private DispatcherTimer _searchTimer;
		bool _isSearchDevicesStarted;

		public bool IsSearchDevicesStarted
		{
			get { return _isSearchDevicesStarted; }
			set
			{
				_isSearchDevicesStarted = value;
				OnPropertyChanged(() => IsSearchDevicesStarted);
			}
		}

		public SearchDevicesViewModel()
		{
			_searchTimer = new DispatcherTimer();
			_searchTimer.Interval = new TimeSpan(0, 0, 7);
			_searchTimer.Tick += _searchTimer_Tick;
			IsSearchDevicesStarted = false;
			SearchDevices = new ObservableCollection<SearchDeviceViewModel>();

			StartSearchDevicesCommand = new RelayCommand(OnStartSearchDevices);
			StopSearchDevicesCommand = new RelayCommand(OnStopSearchDevices);
			AddDeviceCommand = new RelayCommand(OnAddDevice, CanAddDevice);
	
			Wrapper.NewSearchDevice += WrapperOnNewSearchDevice;
		}

		private void _searchTimer_Tick(object sender, EventArgs e)
		{
			OnStopSearchDevices();
		}

		public ObservableCollection<SearchDeviceViewModel> SearchDevices { get; private set; }

		public SearchDeviceViewModel SelectedSearchDevice { get; set; }

		private void WrapperOnNewSearchDevice(DeviceSearchInfo info)
		{
			if (SearchDevices.Any((x) => x.Mac == info.Mac))
				return;

			ApplicationService.BeginInvoke(new Action(() => SearchDevices.Add(new SearchDeviceViewModel
			{
				DeviceType = info.DeviceType,
				IpAddress = info.IpAddress,
				Port = info.Port,
				Submask = info.Submask,
				Gateway = info.Gateway,
				Mac = info.Mac
			})));
		}

		public RelayCommand StartSearchDevicesCommand { get; private set; }
		private void OnStartSearchDevices()
		{
			if (IsSearchDevicesStarted)
				return;

			ApplicationService.BeginInvoke(new Action(() =>
			{
				SearchDevices.Clear();
			}));
			Wrapper.StartSearchDevices();
			_searchTimer.Stop();
			_searchTimer.Start();
			IsSearchDevicesStarted = true;
		}
		private bool CanStartSearchDevices()
		{
			return !IsSearchDevicesStarted;
		}

		public RelayCommand StopSearchDevicesCommand { get; private set; }
		private void OnStopSearchDevices()
		{
			_searchTimer.Stop();
			Wrapper.StopSearchDevices();
			IsSearchDevicesStarted = false;
		}
		private bool CanStopSearchDevices()
		{
			return IsSearchDevicesStarted;
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
