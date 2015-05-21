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
			CableTypes = new ObservableCollection<CableType>(Enum.GetValues(typeof(CableType)).Cast<CableType>());
			_selectedCableType = Device.CableType;
			_cableLenght = Device.CableLength;
		}

		public Device Device { get; private set; }
		public ObservableCollection<CableType> CableTypes { get; private set; }

		CableType _selectedCableType;
		public CableType SelectedCableType
		{
			get { return _selectedCableType; }
			set
			{
				_selectedCableType = value;
				OnPropertyChanged(() => SelectedCableType);
				Device.CableType = value;
			}
		}

		int _cableLenght;
		public int CableLenght
		{
			get { return _cableLenght; }
			set
			{
				_cableLenght = value;
				OnPropertyChanged(() => CableLenght);
				Device.CableLength = value;
			}
		}
	}
}