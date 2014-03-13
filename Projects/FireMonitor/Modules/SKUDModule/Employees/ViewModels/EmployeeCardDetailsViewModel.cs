using System;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class EmployeeCardDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDCard Card { get; private set; }
		public AccessZonesSelectationViewModel AccessZones { get; private set; }
		public AccessZonesSelectationViewModel AdditionalGUDZones { get; private set; }
		public AccessZonesSelectationViewModel ExceptedGUDZones { get; private set; }
		bool IsNewCard;

		public EmployeeCardDetailsViewModel(SKDCard card = null)
		{
			Card = card;
			if (card == null)
			{
				IsNewCard = true;
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
			IDFamily = Card.Series;
			IDNo = Card.Number;
			StartDate = Card.ValidFrom;
			EndDate = Card.ValidTo;

			AccessZones = new AccessZonesSelectationViewModel(Card.CardZones, Card.UID, ParentType.Card);
			AdditionalGUDZones = new AccessZonesSelectationViewModel(Card.AdditionalGUDZones, Card.UID, ParentType.GUDAdditions);
			ExceptedGUDZones = new AccessZonesSelectationViewModel(Card.ExceptedGUDZones, Card.UID, ParentType.GUDExceptons);

			AvailableGUDs = new ObservableCollection<GUD>();
			var guds = GUDHelper.Get(new GUDFilter());
			if (guds != null)
			{
				foreach (var gud in guds)
					AvailableGUDs.Add(gud);
			}
			
			SelectedGUD = AvailableGUDs.FirstOrDefault(x => x.UID == Card.GUDUID);
			StopListCards = new ObservableCollection<SKDCard>();
			var stopListCards = CardHelper.GetStopListCards();
			if (stopListCards == null)
				return;
			foreach (var item in stopListCards)
				StopListCards.Add(item);
			SelectedStopListCard = StopListCards.FirstOrDefault();
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

		ObservableCollection<GUD> _availableGUDs;
		public ObservableCollection<GUD> AvailableGUDs
		{
			get { return _availableGUDs; }
			set
			{
				_availableGUDs = value;
				OnPropertyChanged("AvailableGUDs");
			}
		}

		GUD _selectedGUD;
		public GUD SelectedGUD
		{
			get { return _selectedGUD; }
			set
			{
				_selectedGUD = value;
				OnPropertyChanged("SelectedGUD");
			}
		}

		bool _useStopList;
		public bool UseStopList
		{
			get { return _useStopList; }
			set
			{
				_useStopList = value;
				OnPropertyChanged("UseStopList");
				UpdateStopListCard();
			}
		}

		public ObservableCollection<SKDCard> StopListCards { get; private set; }

		SKDCard _selectedStopListCard;
		public SKDCard SelectedStopListCard
		{
			get { return _selectedStopListCard; }
			set
			{
				_selectedStopListCard = value;
				OnPropertyChanged("SelectedStopListCard");
				UpdateStopListCard();
			}
		}

		void UpdateStopListCard()
		{
			if (UseStopList && SelectedStopListCard != null)
			{
				IDFamily = SelectedStopListCard.Series;
				IDNo = SelectedStopListCard.Number;
			}
		}

		protected override bool Save()
		{
			if (UseStopList)
			{
				if(!IsNewCard)
					CardHelper.ToStopList(Card, "Заменена на карту " + IDFamily + @"\" + IDNo);
				Card.UID = SelectedStopListCard.UID;
				Card.IsInStopList = false;
				Card.StopReason = null;
			}
			Card.Series = IDFamily;
			Card.Number = IDNo;
			Card.ValidFrom = StartDate;
			Card.ValidTo = EndDate;
			Card.CardZones = AccessZones.GetCardZones();
			Card.AdditionalGUDZones = AdditionalGUDZones.GetCardZones();
			Card.ExceptedGUDZones = ExceptedGUDZones.GetCardZones();
			
			if (SelectedGUD != null)
				Card.GUDUID = SelectedGUD.UID;
			return true;
		}
	}
}