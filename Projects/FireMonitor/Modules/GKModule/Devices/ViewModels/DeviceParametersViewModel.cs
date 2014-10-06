using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DeviceParametersViewModel : ViewPartViewModel
	{
		BackgroundWorker BackgroundWorker;
		bool CancelBackgroundWorker = false;

		public DeviceParametersViewModel()
		{
			//ParameterUpdateHelper.NewAUParameterValue -= new Action<AUParameterValue>(ParameterUpdateHelper_NewAUParameterValue);
			//ParameterUpdateHelper.NewAUParameterValue += new Action<AUParameterValue>(ParameterUpdateHelper_NewAUParameterValue);
		}

		public void Initialize()
		{
			Devices = new ObservableCollection<DeviceParameterViewModel>();
			foreach (var device in GKManager.Devices)
			{
				if (device.Driver.MeasureParameters.Where(x => !x.IsDelay).Count() > 0)
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

		int _percentsCompleted;
		public int PercentsCompleted
		{
			get { return _percentsCompleted; }
			set
			{
				_percentsCompleted = value;
				OnPropertyChanged(() => PercentsCompleted);
			}
		}

		void UpdateAuParameters(object sender, DoWorkEventArgs e)
		{
			while (true)
			{
				for (int i = 0; i < Devices.Count; i++)
				{
					var deviceParameterViewModel = Devices[i];
					if (CancelBackgroundWorker)
					{
						Thread.Sleep(TimeSpan.FromSeconds(1));
						continue;
					}

					if (deviceParameterViewModel.Device.Driver.MeasureParameters.Any(x => x.InternalName == "Smokiness"))
						deviceParameterViewModel.Smokiness = "опрос";
					if (deviceParameterViewModel.Device.Driver.MeasureParameters.Any(x => x.InternalName == "Temperature"))
						deviceParameterViewModel.Temperature = "опрос";
					if (deviceParameterViewModel.Device.Driver.MeasureParameters.Any(x => x.InternalName == "Dustinness"))
						deviceParameterViewModel.Dustinness = "опрос";
					if (deviceParameterViewModel.Device.Driver.MeasureParameters.Any(x => x.InternalName == "LastServiceTime"))
						deviceParameterViewModel.LastServiceTime = "опрос";
					if (deviceParameterViewModel.Device.Driver.MeasureParameters.Any(x => x.InternalName == "Resistance"))
						deviceParameterViewModel.Resistance = "опрос";

					deviceParameterViewModel.IsCurrent = true;
					//ParameterUpdateHelper.UpdateDevice(deviceParameterViewModel.Device);
					deviceParameterViewModel.IsCurrent = false;

					if (deviceParameterViewModel.Smokiness == "опрос")
						deviceParameterViewModel.Smokiness = "ошибка";
					if (deviceParameterViewModel.Temperature == "опрос")
						deviceParameterViewModel.Temperature = "ошибка";
					if (deviceParameterViewModel.Dustinness == "опрос")
						deviceParameterViewModel.Dustinness = "ошибка";
					if (deviceParameterViewModel.LastServiceTime == "опрос")
						deviceParameterViewModel.LastServiceTime = "ошибка";
					if (deviceParameterViewModel.Resistance == "опрос")
						deviceParameterViewModel.Resistance = "ошибка";

					PercentsCompleted = ((i + 1) * 100) / Devices.Count;
				}
			}
		}

		void ParameterUpdateHelper_NewAUParameterValue(MeasureParameterViewModel auParameterValue)
		{
			var deviceParameterViewModel = Devices.FirstOrDefault(x => x.Device.UID == auParameterValue.Device.UID);
			if (deviceParameterViewModel != null)
			{
				deviceParameterViewModel.OnNewAUParameterValue(auParameterValue);

				switch (auParameterValue.DriverParameter.InternalName)
				{
					case "Smokiness":
						deviceParameterViewModel.Smokiness = auParameterValue.StringValue;
						break;

					case "Temperature":
						deviceParameterViewModel.Temperature = auParameterValue.StringValue;
						break;

					case "Dustinness":
						deviceParameterViewModel.Dustinness = auParameterValue.StringValue;
						break;

					case "LastServiceTime":
						deviceParameterViewModel.LastServiceTime = auParameterValue.StringValue;
						break;

					case "Resistance":
						deviceParameterViewModel.Resistance = auParameterValue.StringValue;
						break;
				}
			}
		}

		public override void OnShow()
		{
			CancelBackgroundWorker = false;
			foreach (var device in Devices)
			{
				device.IsCurrent = false;
			}

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