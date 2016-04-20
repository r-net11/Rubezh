using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows.Windows.ViewModels;
using PowerCalculator.Models;
using PowerCalculator.Processor;
using System.Collections.Generic;
using System;

namespace PowerCalculator.ViewModels
{
	public class NewDeviceViewModel : SaveCancelDialogViewModel
	{
		public NewDeviceViewModel()
		{
			Title = "Добавление нового устройства";
			Drivers = new ObservableCollection<Driver>(DriversHelper.Drivers.Where(x=>x.CanAdd));
			SelectedDriver = Drivers.FirstOrDefault();
			CableResistivity = 1;
			CableLength = 1;
			Count = 1;
		}

		public ObservableCollection<Driver> Drivers { get; private set; }

		Driver _selectedDriver;
		public Driver SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				OnPropertyChanged(() => SelectedDriver);
			}
		}

        public List<CableType> CableTypes { get { return CableTypesRepository.CableTypes; } }

        CableType _selectedCableType;
        public CableType SelectedCableType
        {
            get { return _selectedCableType; }
            set
            {
                _selectedCableType = value;
                OnPropertyChanged(() => SelectedCableType);

                if (_selectedCableType != CableTypesRepository.CustomCableType)
                    CableResistivity = _selectedCableType.Resistivity;
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
                if (_selectedCableType == null || _cableResistivity != _selectedCableType.Resistivity)
                    SelectedCableType = CableTypesRepository.CustomCableType;
			}
		}

		double _cableLength;
		public double CableLength
		{
			get { return _cableLength; }
			set
			{
                _cableLength = value > 0 ? Math.Round(value, 2) : 1;
                if (_cableLength > 1000)
                    _cableLength = 1000;
				OnPropertyChanged(() => CableLength);
			}
		}

		int _count;
		public int Count
		{
			get { return _count; }
			set
			{
				_count = value;
				OnPropertyChanged(() => Count);
			}
		}
	}
}