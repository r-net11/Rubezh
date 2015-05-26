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
			CableTypes = new ObservableCollection<Cable>(Enum.GetValues(typeof(Cable)).Cast<Cable>());
			_selectedCableType = Device.CableType;
			_cableLenght = Device.CableLength;
		}

		public Device Device { get; private set; }
		public ObservableCollection<Cable> CableTypes { get; private set; }

		Cable _selectedCableType;
		public Cable SelectedCableType
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