using System;
using System.Collections.ObjectModel;
using System.Linq;
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
			_cableLenght = Device.Cable.Length;
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

		double _cableLenght;
		public double CableLenght
		{
			get { return _cableLenght; }
			set
			{
				_cableLenght = value;
				OnPropertyChanged(() => CableLenght);
				Device.Cable.Length = value;
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
	}
}