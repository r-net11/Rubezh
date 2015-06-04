using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;

namespace PowerCalculator.ViewModels
{
	public class CableSpecificationItemViewModel : BaseViewModel
	{
		public CableSpecificationItem CableSpecificationItem { get; private set; }

		public CableSpecificationItemViewModel(CableSpecificationItem cableSpecificationItem)
		{
			CableSpecificationItem = cableSpecificationItem;
			_resistivity = cableSpecificationItem.Resistivity;
			_length = cableSpecificationItem.Length;
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
                    _resistivity = value;
				OnPropertyChanged(() => Resistivity);
				CableSpecificationItem.Resistivity = _resistivity;
			}
		}

		double _length;
		public double Length
		{
			get { return _length; }
			set
			{
                _length = value > 0 ? value : 1;
				OnPropertyChanged(() => Length);
				CableSpecificationItem.Length = _length;
			}
		}
	}
}