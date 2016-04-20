using Infrastructure.Common.Windows.Windows.ViewModels;
using PowerCalculator.Models;
using PowerCalculator.Processor;
using System;
using System.Collections.Generic;

namespace PowerCalculator.ViewModels
{
	public class CableSpecificationItemViewModel : BaseViewModel
	{
		public CableSpecificationItem CableSpecificationItem { get; private set; }

		public CableSpecificationItemViewModel(CableSpecificationItem cableSpecificationItem)
		{
			CableSpecificationItem = cableSpecificationItem;
            _cableType = cableSpecificationItem.CableType;
			_resistivity = cableSpecificationItem.Resistivity;
			_length = cableSpecificationItem.Length;
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
                CableSpecificationItem.CableType = _cableType;
                if (_cableType != CableTypesRepository.CustomCableType)
                    Resistivity = _cableType.Resistivity;
            }
        }

		double _resistivity;
		public double Resistivity
		{
			get { return _resistivity; }
			set
			{
                if (value <= 0)
                    _resistivity = 1;
                else if (value > 10)
                    _resistivity = 10;
                else
                    _resistivity = Math.Round(value, 5);
				OnPropertyChanged(() => Resistivity);
				CableSpecificationItem.Resistivity = _resistivity;
                if (_cableType == null || _resistivity != _cableType.Resistivity)
                    CableType = CableTypesRepository.CustomCableType;
			}
		}

		double _length;
		public double Length
		{
			get { return _length; }
			set
			{
                _length = value > 0 ? Math.Round(value, 2) : 1;
				OnPropertyChanged(() => Length);
				CableSpecificationItem.Length = _length;
			}
		}
	}
}