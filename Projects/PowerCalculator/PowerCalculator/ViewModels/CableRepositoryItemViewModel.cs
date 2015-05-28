using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;

namespace PowerCalculator.ViewModels
{
	public class CableRepositoryItemViewModel : BaseViewModel
	{
		public CableRepositoryItem CableRepositoryItem { get; private set; }

		public CableRepositoryItemViewModel(CableRepositoryItem cableRepositoryItem)
		{
			CableRepositoryItem = cableRepositoryItem;
			_resistivity = cableRepositoryItem.Resistivity;
			_lenght = cableRepositoryItem.Length;
		}

		double _resistivity;
		public double Resistivity
		{
			get { return _resistivity; }
			set
			{
				_resistivity = value;
				OnPropertyChanged(() => Resistivity);
				CableRepositoryItem.Resistivity = value;
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
				CableRepositoryItem.Length = value;
			}
		}
	}
}