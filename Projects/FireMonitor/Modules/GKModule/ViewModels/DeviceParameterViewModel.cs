using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.ObjectModel;

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

			Smokiness = " - ";
			Temperature = " - ";
			Dustinness = " - ";
			LastServiceTime = " - ";
			Resistance = " - ";
			AUParameterValues = new ObservableCollection<AUParameterValue>();
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

		string _smokiness;
		public string Smokiness
		{
			get { return _smokiness; }
			set
			{
				_smokiness = value;
				OnPropertyChanged("Smokiness");
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

		public void OnNewAUParameterValue(AUParameterValue value)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				var auParameterValue = AUParameterValues.FirstOrDefault(x => x.Name == value.Name);
				if (auParameterValue == null)
				{
					auParameterValue = value;
					AUParameterValues.Add(auParameterValue);
				}
				auParameterValue.Value = value.Value;
				auParameterValue.StringValue = value.StringValue;
				OnPropertyChanged("AUParameterValues");
			}));
		}

		ObservableCollection<AUParameterValue> _auParameterValues;
		public ObservableCollection<AUParameterValue> AUParameterValues
		{
			get { return _auParameterValues; }
			set
			{
				_auParameterValues = value;
				OnPropertyChanged("AUParameterValues");
			}
		}
	}
}