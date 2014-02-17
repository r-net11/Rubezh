using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using System.Linq;
using FiresecClient.SKDHelpers;
using System.Collections.Generic;
using System;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class UserAccessViewModel : TreeNodeViewModel<ZoneViewModel>
	{
		public Employee Employee { get; private set; }

		public UserAccessViewModel(Employee employee)
		{
			Employee = employee;
			AddCardCommand = new RelayCommand(OnAddCard, CanAddCard);
			RemoveCardCommand = new RelayCommand(OnRemoveCard, CanRemoveCard);

			var filter = new CardFilter{ EmployeeUids = new List<Guid>() { Employee.UID } };
			var cards = FiresecManager.GetCards(filter);
			Cards = new ObservableCollection<EmployeeCardViewModel>();
			foreach (var item in cards)
			{
				Cards.Add(new EmployeeCardViewModel(this, item));
			}
			SelectedCard = Cards.FirstOrDefault();
		}

		public ObservableCollection<EmployeeCardViewModel> Cards { get; private set; }

		EmployeeCardViewModel _selectedCard;
		public EmployeeCardViewModel SelectedCard
		{
			get { return _selectedCard; }
			set
			{
				_selectedCard = value;
				OnPropertyChanged("SelectedCard");
			}
		}

		public RelayCommand AddCardCommand { get; private set; }
		void OnAddCard()
		{
			var cardDetailsViewModel = new EmployeeCardDetailsViewModel();
			if (DialogService.ShowModalWindow(cardDetailsViewModel))
			{
				var card = cardDetailsViewModel.Card;
				CardHelper.LinkToEmployee(card, Employee.UID);
				var cardViewModel = new EmployeeCardViewModel(this, card);
				Cards.Add(cardViewModel);
				SelectedCard = cardViewModel;
			}
		}
		public bool CanAddCard()
		{ 
			return Cards.Count < 10;
		}

		public RelayCommand RemoveCardCommand { get; private set; }
		void OnRemoveCard()
		{
			var cardRemovalReasonViewModel = new CardRemovalReasonViewModel();
			if (DialogService.ShowModalWindow(cardRemovalReasonViewModel))
			{
				var cardRemovalReason = cardRemovalReasonViewModel.CardRemovalReason;
				var card = Cards.FirstOrDefault();
				Cards.Remove(card);
				CardHelper.ToStopList(card.Card, cardRemovalReason);
			}
		}
		public bool CanRemoveCard()
		{
			return SelectedCard != null;
		}
	}
}