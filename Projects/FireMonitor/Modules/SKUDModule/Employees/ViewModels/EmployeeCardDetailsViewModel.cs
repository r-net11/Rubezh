using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

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
			Series = Card.Series;
			Number = Card.Number;
			StartDate = Card.ValidFrom;
			EndDate = Card.ValidTo;

			AccessZones = new AccessZonesSelectationViewModel(Organization, Card.CardZones, Card.UID);

			AvailableAccessTemplates = new ObservableCollection<AccessTemplate>();
			AvailableAccessTemplates.Add(new AccessTemplate() { Name = "НЕТ" });
			var accessTemplateFilter = new AccessTemplateFilter();
			accessTemplateFilter.OrganizationUIDs.Add(Organization.UID);
			var accessTemplates = AccessTemplateHelper.Get(accessTemplateFilter);
			if (accessTemplates != null)
			{
				foreach (var accessTemplate in accessTemplates)
					AvailableAccessTemplates.Add(accessTemplate);
			}

			SelectedAccessTemplate = AvailableAccessTemplates.FirstOrDefault(x => x.UID == Card.AccessTemplateUID);
			StopListCards = new ObservableCollection<SKDCard>();
			var stopListCards = CardHelper.GetStopListCards();
			if (stopListCards == null)
				return;
			foreach (var item in stopListCards)
				StopListCards.Add(item);
			SelectedStopListCard = StopListCards.FirstOrDefault();
		}

		int _series;
		public int Series
		{
			get { return _series; }
			set
			{
				_series = value;
				OnPropertyChanged("Series");
			}
		}

		int _number;
		public int Number
		{
			get { return _number; }
			set
			{
				_number = value;
				OnPropertyChanged("Number");
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

		ObservableCollection<AccessTemplate> _availableAccessTemplates;
		public ObservableCollection<AccessTemplate> AvailableAccessTemplates
		{
			get { return _availableAccessTemplates; }
			set
			{
				_availableAccessTemplates = value;
				OnPropertyChanged("AvailableAccessTemplates");
			}
		}

		AccessTemplate _selectedAccessTemplate;
		public AccessTemplate SelectedAccessTemplate
		{
			get { return _selectedAccessTemplate; }
			set
			{
				_selectedAccessTemplate = value;
				OnPropertyChanged("SelectedAccessTemplate");
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
				Series = SelectedStopListCard.Series;
				Number = SelectedStopListCard.Number;
			}
		}

		protected override bool Save()
		{
			if (UseStopList && SelectedStopListCard != null)
			{
				if (!IsNewCard)
					CardHelper.ToStopList(Card, "Заменена на карту " + Series + @"\" + Number);
				Card.UID = SelectedStopListCard.UID;
				Card.IsInStopList = false;
				Card.StopReason = null;
			}
			Card.Series = Series;
			Card.Number = Number;
			Card.ValidFrom = StartDate;
			Card.ValidTo = EndDate;
			Card.CardZones = AccessZones.GetCardZones();

			if (SelectedAccessTemplate != null)
				Card.AccessTemplateUID = SelectedAccessTemplate.UID;
			if (AvailableAccessTemplates.IndexOf(SelectedAccessTemplate) == 0)
				Card.AccessTemplateUID = null;
			return true;
		}
	}
}