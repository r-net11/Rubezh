using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;

namespace PowerCalculator.ViewModels
{
	public class DeviceViewModel : BaseViewModel
	{
		public DeviceViewModel(Device device, LineViewModel owner)
		{
			Device = device;
            Owner = owner;
			_cableResistivity = Device.Cable.Resistivity;
			_cableLength = Device.Cable.Length;
		}

		public Device Device { get; private set; }
        public LineViewModel Owner { get; private set; }

		double _cableResistivity;
		public double CableResistivity
		{
			get { return _cableResistivity; }
			set
			{
                if (value <= 0)
                    _cableResistivity = 1;
                else if (value > 10)
                    _cableResistivity = 10;
                else
                    _cableResistivity = value;

				OnPropertyChanged(() => CableResistivity);
                Device.Cable.Resistivity = _cableResistivity;
                if (Owner != null)
                    Owner.Calculate();
			}
		}

		double _cableLength;
		public double CableLength
		{
			get { return _cableLength; }
			set
			{
                _cableLength = value > 0 ? value : 1;

				OnPropertyChanged(() => CableLength);
                Device.Cable.Length = _cableLength;
                if (Owner != null)
                    Owner.Calculate();
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

		bool _hasIError;
		public bool HasIError
		{
			get { return _hasIError; }
			set
			{
				_hasIError = value;
				OnPropertyChanged(() => HasIError);
			}
		}

        bool _hasUError;
        public bool HasUError
        {
            get { return _hasUError; }
            set
            {
                _hasUError = value;
                OnPropertyChanged(() => HasUError);
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