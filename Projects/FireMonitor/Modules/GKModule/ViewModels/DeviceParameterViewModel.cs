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
			LastServiceTime = " - ";
			Resistance = " - ";
		}

		bool _isCurrent;
		public bool IsCurrent
		{
			get { return _isCurrent; }
			set
			{
				_isCurrent = value;
				OnPropertyChanged("IsCurrent");
			}
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

		string _lastServiceTime;
		public string LastServiceTime
		{
			get { return _lastServiceTime; }
			set
			{
				_lastServiceTime = value;
				OnPropertyChanged("LastServiceTime");
			}
		}

		string _resistance;
		public string Resistance
		{
			get { return _resistance; }
			set
			{
				_resistance = value;
				OnPropertyChanged("Resistance");
			}
		}
	}
}