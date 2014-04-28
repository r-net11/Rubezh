using FiresecAPI;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using System;
using System.Linq;

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
			AvaivableDeletedTypes = new ObservableCollection<DeletedType>(Enum.GetValues(typeof(DeletedType)).OfType<DeletedType>());
			SelectedDeletedType = cardFilter.WithDeleted;
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

		public ObservableCollection<DeletedType> AvaivableDeletedTypes { get; private set; }

		DeletedType _selectedDeletedType;
		public DeletedType SelectedDeletedType
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
			cardFilter.WithDeleted = SelectedDeletedType;
			return cardFilter;
		}
	}
}