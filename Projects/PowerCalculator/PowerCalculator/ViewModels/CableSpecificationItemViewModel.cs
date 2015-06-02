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
			_lenght = cableSpecificationItem.Length;
		}

		double _resistivity;
		public double Resistivity
		{
			get { return _resistivity; }
			set
			{
				_resistivity = value;
				OnPropertyChanged(() => Resistivity);
				CableSpecificationItem.Resistivity = value;
			}
		}

		double _lenght;
		public double Lenght
		{
			get { return _lenght; }
			set
			{
				_lenght = value;
				OnPropertyChanged(() => Lenght);
				CableSpecificationItem.Length = value;
			}
		}
	}
}