using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using FiresecClient;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace GKModule.ViewModels
{
	public class DeviceParametersViewModel : ViewPartViewModel
	{
		BackgroundWorker BackgroundWorker;
		bool CancelBackgroundWorker = false;

		public DeviceParametersViewModel()
		{
		}

		public void Initialize()
		{
			Devices = new ObservableCollection<DeviceParameterViewModel>();
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.AUParameters.Where(x => !x.IsDelay).Count() > 0)
				{
					var deviceParameterViewModel = new DeviceParameterViewModel(device);
					Devices.Add(deviceParameterViewModel);
				}
			}
			SelectedDevice = Devices.FirstOrDefault();
		}

		public ObservableCollection<DeviceParameterViewModel> Devices { get; private set; }

		DeviceParameterViewModel _selectedDevice;
		public DeviceParameterViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}

		public override void OnShow()
		{
			BackgroundWorker = new BackgroundWorker();
			BackgroundWorker.DoWork += new DoWorkEventHandler(UpdateAuParameters);
			BackgroundWorker.RunWorkerAsync();
		}

		public override void OnHide()
		{
			CancelBackgroundWorker = true;
		}

		void UpdateAuParameters(object sender, DoWorkEventArgs e)
		{
			while (true)
			{
				if (CancelBackgroundWorker)
					break;
				foreach (var deviceParameterViewModel in Devices)
				{
					ParameterUpdateHelper.NewAUParameterValue += new Action<AUParameterValue>(ParameterUpdateHelper_NewAUParameterValue);
					ParameterUpdateHelper.UpdateDevice(deviceParameterViewModel.Device);
					ParameterUpdateHelper.NewAUParameterValue -= new Action<AUParameterValue>(ParameterUpdateHelper_NewAUParameterValue);
					Thread.Sleep(TimeSpan.FromSeconds(1));
				}
			}
		}

		void ParameterUpdateHelper_NewAUParameterValue(AUParameterValue auParameterValue)
		{
			Trace.WriteLine("AUParameterValue " + auParameterValue.Device.ShortNameAndDottedAddress + " - " + auParameterValue.Name + " - " + auParameterValue.Value);
		}
	}
}