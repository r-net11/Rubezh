using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using RubezhClient;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.GK;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class DeviceParametersViewModel : ViewPartViewModel
	{
		BackgroundWorker BackgroundWorker;
		bool CancelBackgroundWorker = false;

		public void Initialize()
		{
			Devices = new ObservableCollection<DeviceParameterViewModel>();
			foreach (var device in GKManager.Devices)
			{
				if (device.Driver.MeasureParameters.Count(x => !x.IsDelay) > 0)
				{
					var deviceParameterViewModel = new DeviceParameterViewModel(device);
					Devices.Add(deviceParameterViewModel);
				}
			}
			SelectedDevice = Devices.FirstOrDefault();
		}

		ObservableCollection<DeviceParameterViewModel> _devices;
		public ObservableCollection<DeviceParameterViewModel> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged(() => Devices);
			}
		}

		DeviceParameterViewModel _selectedDevice;
		public DeviceParameterViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		void UpdateAuParameters(object sender, DoWorkEventArgs e)
		{
			while (true)
			{
				foreach (var deviceParameterViewModel in Devices)
				{
					if (CancelBackgroundWorker)
					{
						Thread.Sleep(TimeSpan.FromSeconds(1));
						continue;
					}

					ClientManager.FiresecService.GKStartMeasureMonitoring(deviceParameterViewModel.Device);
				}

				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
		}

		public override void OnShow()
		{
			CancelBackgroundWorker = false;
			if (BackgroundWorker == null)
			{
				BackgroundWorker = new BackgroundWorker();
				BackgroundWorker.DoWork += new DoWorkEventHandler(UpdateAuParameters);
				BackgroundWorker.RunWorkerAsync();
			}
		}

		public override void OnHide()
		{
			CancelBackgroundWorker = true;
		}
	}
}