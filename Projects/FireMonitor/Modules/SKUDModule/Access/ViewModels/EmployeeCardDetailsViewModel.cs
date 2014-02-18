using System;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeCardDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDCard Card { get; private set; }
		public AccessZonesSelectationViewModel AccessZonesSelectationViewModel { get; private set; }

		public EmployeeCardDetailsViewModel(SKDCard card = null)
		{
			Card = card;
			if (card == null)
			{
				Title = "Создание карты";
				card = new SKDCard()
				{
					Series = 0,
					Number = 0,
					ValidFrom = DateTime.Now,
					ValidTo = DateTime.Now.AddYears(1)
				};
			}
			else
			{
				Title = string.Format("Свойства карты: {0}", card.Series + "/" + card.Number);
			}
			Card = card;
			if (Card.Series.HasValue)
				IDFamily = Card.Series.Value;
			if (Card.Number.HasValue)
				IDNo = Card.Number.Value;
			if (Card.ValidFrom.HasValue)
				StartDate = Card.ValidFrom.Value;
			if (Card.ValidTo.HasValue)
				EndDate = Card.ValidTo.Value;

			AccessZonesSelectationViewModel = new AccessZonesSelectationViewModel(Card.CardZones);
		}

		int _idFamily;
		public int IDFamily
		{
			get { return _idFamily; }
			set
			{
				_idFamily = value;
				OnPropertyChanged("IDFamily");
			}
		}

		int _idNo;
		public int IDNo
		{
			get { return _idNo; }
			set
			{
				_idNo = value;
				OnPropertyChanged("IDNo");
			}
		}

		DateTime _startDate;
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				OnPropertyChanged("StartDate");
			}
		}

		DateTime _endDate;
		public DateTime EndDate
		{
			get { return _endDate; }
			set
			{
				_endDate = value;
				OnPropertyChanged("EndDate");
			}
		}

		protected override bool Save()
		{
			Card.Series = IDFamily;
			Card.Number = IDNo;
			Card.ValidFrom = StartDate;
			Card.ValidTo = EndDate;
			Card.CardZones = AccessZonesSelectationViewModel.GetCardZones();
			return true;
		}
	}
}