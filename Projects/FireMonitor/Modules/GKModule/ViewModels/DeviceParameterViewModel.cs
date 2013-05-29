using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DeviceParameterViewModel : BaseViewModel
	{
		public DeviceViewModel DeviceViewModel { get; private set; }
		public XDevice Device { get; private set; }

		public DeviceParameterViewModel(XDevice device)
		{
			Device = device;
			DeviceViewModel = new DeviceViewModel(device);

			Temperature = " - ";
			Dustinness = " - ";
		}

		string _temperature;
		public string Temperature
		{
			get { return _temperature; }
			set
			{
				_temperature = value;
				OnPropertyChanged("Temperature");
			}
		}

		string _dustinness;
		public string Dustinness
		{
			get { return _dustinness; }
			set
			{
				_dustinness = value;
				OnPropertyChanged("Dustinness");
			}
		}
	}
}