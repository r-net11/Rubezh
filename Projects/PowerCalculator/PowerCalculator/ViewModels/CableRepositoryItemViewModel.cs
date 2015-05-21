using System;
using System.Collections.ObjectModel;
using System.Linq;
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

			CableTypes = new ObservableCollection<CableTypeViewModel>(Enum.GetValues(typeof(CableType)).Cast<CableType>().Select(x => new CableTypeViewModel(x)));
			_selectedCableType = CableTypes.FirstOrDefault(x => x.CableType == cableRepositoryItem.CableType);
			_lenght = cableRepositoryItem.Lenght;
		}

		public ObservableCollection<CableTypeViewModel> CableTypes { get; private set; }

		CableTypeViewModel _selectedCableType;
		public CableTypeViewModel SelectedCableType
		{
			get { return _selectedCableType; }
			set
			{
				_selectedCableType = value;
				OnPropertyChanged(() => SelectedCableType);
				CableRepositoryItem.CableType = value.CableType;
			}
		}

		int _lenght;
		public int Lenght
		{
			get { return _lenght; }
			set
			{
				_lenght = value;
				OnPropertyChanged(() => Lenght);
				CableRepositoryItem.Lenght = value;
			}
		}
	}
}