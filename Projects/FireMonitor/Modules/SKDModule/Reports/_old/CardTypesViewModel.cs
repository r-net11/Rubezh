using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common;
using Infrastructure.Common.CheckBoxList;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class CardTypesViewModel : BaseViewModel
	{
		public CheckBoxItemList<CardTypeViewModel> CardTypes { get; private set; }

		public CardTypesViewModel()
		{
			SelectAllCommand = new RelayCommand(OnSelectAll, () => CanSelect);
			SelectNoneCommand = new RelayCommand(OnSelectNone, () => CanSelect);

			CardTypes = new CheckBoxItemList<CardTypeViewModel>();
			foreach (CardType cardType in Enum.GetValues(typeof(CardType)))
				CardTypes.Add(new CardTypeViewModel(cardType));
		}

		public void Update(CardReportFilter cardReportFilter)
		{
			foreach (var cardType in CardTypes.Items)
				cardType.IsChecked = false;
			var checkedCardTypes = CardTypes.Items.Where(x => cardReportFilter.CardFilter.CardTypes.Any(y => y == x.CardType));
			foreach (var cardType in checkedCardTypes)
				cardType.IsChecked = true;
		}

		public List<CardType> GetCheckedCardTypes()
		{
			return CardTypes.Items.Where(x => x.IsChecked).Select(x => x.CardType).ToList();
		}

		public RelayCommand SelectAllCommand { get; private set; }
		void OnSelectAll()
		{
			foreach (var item in CardTypes.Items)
				item.IsChecked = true;
		}

		public RelayCommand SelectNoneCommand { get; private set; }
		void OnSelectNone()
		{
			foreach (var item in CardTypes.Items)
				item.IsChecked = false;
		}

		bool _canSelect;
		public bool CanSelect
		{
			get { return _canSelect; }
			set
			{
				_canSelect = value;
				OnPropertyChanged(() => CanSelect);
			}
		}
	}
}
