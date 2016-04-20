using Infrastructure.Common.Windows.Windows.ViewModels;
using PowerCalculator.Models;
using PowerCalculator.Processor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PowerCalculator.ViewModels
{
	public class DeviceViewModel : BaseViewModel
	{
        public Device Device { get; private set; }
        public LineViewModel Owner { get; private set; }

        public DeviceViewModel(Device device, LineViewModel owner)
		{
			Device = device;
            Owner = owner;
            _cableType = Device.Cable.CableType;
			_cableResistivity = Device.Cable.Resistivity;
			_cableLength = Device.Cable.Length;

            Drivers = new ObservableCollection<DriverViewModel>(DriversHelper.Drivers.Where(x => x.CanAdd).Select(x => new DriverViewModel(x)));
            _selectedDriver = Drivers.FirstOrDefault(x => x.Driver.DriverType == Device.DriverType);
		}
        
        public ObservableCollection<DriverViewModel> Drivers { get; private set; }

        DriverViewModel _selectedDriver;
        public DriverViewModel SelectedDriver
        {
            get { return _selectedDriver; }
            set
            {
                _selectedDriver = value;
                OnPropertyChanged(() => SelectedDriver);
                if (Owner != null)
                    Owner.ChangeDriver(this, value.Driver.DriverType);   
            }
        }

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
                    _cableResistivity = Math.Round(value, 5);

				OnPropertyChanged(() => CableResistivity);
                Device.Cable.Resistivity = _cableResistivity;
                if (_cableType == null || _cableResistivity != _cableType.Resistivity)
                    CableType = CableTypesRepository.CustomCableType;
                if (Owner != null)
                    Owner.Calculate();
			}
		}

        public List<CableType> CableTypes { get { return CableTypesRepository.CableTypes; } }
        CableType _cableType;
        public CableType CableType
        {
            get { return _cableType; }
            set
            {
                _cableType = value;
                OnPropertyChanged(() => CableType);
                Device.Cable.CableType = _cableType;
                if (_cableType != CableTypesRepository.CustomCableType)
                    CableResistivity = _cableType.Resistivity;
            }
        }

		double _cableLength;
		public double CableLength
		{
            get { return Math.Round(_cableLength, 2); }
			set
			{
                _cableLength = value > 0 ? value : 1;
                if (_cableLength > 1000)
                    _cableLength = 1000;

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
            get { return Math.Round(_current, 2); }
			set
			{
				_current = value;
				OnPropertyChanged(() => Current);
			}
		}

		double _voltage;
		public double Voltage
		{
            get { return Math.Round(_voltage, 2); }
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
	}
}