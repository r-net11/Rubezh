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
		Organization Organization;
		public SKDCard Card { get; private set; }
		public AccessZonesSelectationViewModel AccessZones { get; private set; }
		bool IsNewCard;

		public EmployeeCardDetailsViewModel(Organization organization, SKDCard card = null)
		{
			Organization = organization;
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
				Title = string.Format("Свойства карты: {0}", card.PresentationName);
			}
			Card = card;
			IDFamily = Card.Series;
			IDNo = Card.Number;
			StartDate = Card.ValidFrom;
			EndDate = Card.ValidTo;

			AccessZones = new AccessZonesSelectationViewModel(Organization, Card.CardZones, Card.UID, ParentType.Card);

			AvailableGUDs = new ObservableCollection<GUD>();
			AvailableGUDs.Add(new GUD() { Name = "НЕТ" });
			var gudFilter = new GUDFilter();
			gudFilter.OrganizationUIDs.Add(Organization.UID);
			var guds = GUDHelper.Get(gudFilter);
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
			if (UseStopList && SelectedStopListCard != null)
			{
				if (!IsNewCard)
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

			if (SelectedGUD != null)
				Card.GUDUID = SelectedGUD.UID;
			if (AvailableGUDs.IndexOf(SelectedGUD) == 0)
				Card.GUDUID = null;
			return true;
		}
	}
}