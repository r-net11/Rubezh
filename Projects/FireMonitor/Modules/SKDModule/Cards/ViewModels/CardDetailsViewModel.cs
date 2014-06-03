using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class CardDetailsViewModel : SaveCancelDialogViewModel
	{
		public List<SKDCard> Cards { get; private set; }

		public CardDetailsViewModel()
		{
			Title = "Создание пропуска";
			Cards = new List<SKDCard>();
		}

		int _series;
		public int Series
		{
			get { return _series; }
			set
			{
				if (_series != value)
				{
					_series = value;
					OnPropertyChanged("Series");
				}
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

		protected override bool CanSave()
		{
			return true;
		}

		protected override bool Save()
		{
			for (int i = FirstNos; i < LastNos; i++)
			{
				var card = new SKDCard()
				{
					Series = Series,
					Number = i,
					IsInStopList = true,
				};
				var saveResult = CardHelper.Save(card, false);
				if (saveResult)
				{
					Cards.Add(card);
				}
			}

			return Cards.Count > 0;
		}
	}
}