using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;

namespace PowerCalculator.ViewModels
{
	public class DeviceViewModel : BaseViewModel
	{
		public DeviceViewModel(Device device)
		{
			Device = device;
			_cableResistivity = Device.Cable.Resistivity;
			_cableLength = Device.Cable.Length;
		}

		public Device Device { get; private set; }

		double _cableResistivity;
		public double CableResistivity
		{
			get { return _cableResistivity; }
			set
			{
				_cableResistivity = value;
				OnPropertyChanged(() => CableResistivity);
				Device.Cable.Resistivity = value;
			}
		}

		double _cableLength;
		public double CableLength
		{
			get { return _cableLength; }
			set
			{
				_cableLength = value;
				OnPropertyChanged(() => CableLength);
				Device.Cable.Length = value;
			}
		}

		uint _address;
		public uint Address
		{
			get { return _address; }
			set
			{
				_address = value;
				OnPropertyChanged(() => Address);

				if(Device.Driver.Mult > 1)
					PresentationAddress = Address.ToString() + " - " + (Address + Device.Driver.Mult - 1).ToString();
				else
					PresentationAddress = Address.ToString();
			}
		}

		string _presentationAddress;
		public string PresentationAddress
		{
			get { return _presentationAddress; }
			set
			{
				_presentationAddress = value;
				OnPropertyChanged(() => PresentationAddress);
			}
		}

		ErrorType _errorType;
		public ErrorType ErrorType
		{
			get { return _errorType; }
			set
			{
				_errorType = value;
				OnPropertyChanged(() => ErrorType);
			}
		}

		double _errorScale;
		public double ErrorScale
		{
			get { return _errorScale; }
			set
			{
				_errorScale = value;
				OnPropertyChanged(() => ErrorScale);
			}
		}

		double _current;
		public double Current
		{
			get { return _current; }
			set
			{
				_current = value;
				OnPropertyChanged(() => Current);
			}
		}

		double _voltage;
		public double Voltage
		{
			get { return _voltage; }
			set
			{
				_voltage = value;
				OnPropertyChanged(() => Voltage);
			}
		}

		bool _hasError;
		public bool HasError
		{
			get { return _hasError; }
			set
			{
				_hasError = value;
				OnPropertyChanged(() => HasError);
			}
		}
	}
}