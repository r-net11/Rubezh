using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class CardFilterViewModel : BaseViewModel
	{
		public CardFilterViewModel(CardFilter cardFilter)
		{
			FirstSeries = cardFilter.FirstSeries;
			LastSeries = cardFilter.LastSeries;
			FirstNos = cardFilter.FirstNos;
			LastNos = cardFilter.LastNos;
			AvaivableDeletedTypes = new ObservableCollection<LogicalDeletationType>(Enum.GetValues(typeof(LogicalDeletationType)).OfType<LogicalDeletationType>());
			SelectedDeletedType = cardFilter.DeactivationType;
		}

		int _firstSeries;
		public int FirstSeries
		{
			get { return _firstSeries; }
			set
			{
				_firstSeries = value;
				OnPropertyChanged(() => FirstSeries);
			}
		}

		int _lastSeries;
		public int LastSeries
		{
			get { return _lastSeries; }
			set
			{
				_lastSeries = value;
				OnPropertyChanged(() => LastSeries);
			}
		}

		int _firstNos;
		public int FirstNos
		{
			get { return _firstNos; }
			set
			{
				_firstNos = value;
				OnPropertyChanged(() => FirstNos);
			}
		}

		int _lastNos;
		public int LastNos
		{
			get { return _lastNos; }
			set
			{
				_lastNos = value;
				OnPropertyChanged(() => LastNos);
			}
		}

		public ObservableCollection<LogicalDeletationType> AvaivableDeletedTypes { get; private set; }

		LogicalDeletationType _selectedDeletedType;
		public LogicalDeletationType SelectedDeletedType
		{
			get { return _selectedDeletedType; }
			set
			{
				_selectedDeletedType = value;
				OnPropertyChanged("SelectedDeletedType");
			}
		}

		public CardFilter Save()
		{
			var cardFilter = new CardFilter();
			cardFilter.FirstSeries = FirstSeries;
			cardFilter.LastSeries = LastSeries;
			cardFilter.FirstNos = FirstNos;
			cardFilter.LastNos = LastNos;
			cardFilter.DeactivationType = SelectedDeletedType;
			return cardFilter;
		}
	}
}