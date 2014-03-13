using System;
using System.Collections.Generic;
using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class EmployeeCardViewModel : BaseViewModel
	{
		public SKDCard Card { get; private set; }
		EmployeeViewModel EmployeeViewModel;
		List<Guid> ZoneUIDs;
		public CardZonesViewModel CardZonesViewModel { get; private set; }

		public EmployeeCardViewModel(EmployeeViewModel employeeViewModel, SKDCard card)
		{
			RemoveCommand = new RelayCommand(OnRemove);
			EditCommand = new RelayCommand(OnEdit);
			ShowCardCommand = new RelayCommand(OnShowCard);

			EmployeeViewModel = employeeViewModel;
			Card = card;
			ZoneUIDs = new List<Guid>();
			CardZonesViewModel = new CardZonesViewModel(Card.CardZones);
		}

		bool _isBlocked;
		public bool IsBlocked
		{
			get { return _isBlocked; }
			set
			{
				_isBlocked = value;
				OnPropertyChanged("IsBlocked");
			}
		}

		public string ID
		{
			get { return Card.Series + "/" + Card.Number; }
		}
		public DateTime StartDate
		{
			get { return Card.ValidFrom; }
		}
		public DateTime EndDate
		{
			get { return Card.ValidTo; }
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			EmployeeViewModel.RemoveCard(this);
			EmployeesViewModel.Current.SelectedEmployee = EmployeesViewModel.Current.SelectedEmployee;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var employeeCardDetailsViewModel = new EmployeeCardDetailsViewModel(Card);
			if (DialogService.ShowModalWindow(employeeCardDetailsViewModel))
			{
				var card = employeeCardDetailsViewModel.Card;
				var saveResult = CardHelper.Save(card);
				if (!saveResult)
					return;
				Card = card;
				OnPropertyChanged("ID");
				OnPropertyChanged("StartDate");
				OnPropertyChanged("EndDate");
				CardZonesViewModel.Update();
			}
		}

		public RelayCommand ShowCardCommand { get; private set; }
		void OnShowCard()
		{
			EmployeesViewModel.Current.SelectedCard = this;
		}

		bool _isBold;
		public bool IsBold
		{
			get { return _isBold; }
			set
			{
				_isBold = value;
				OnPropertyChanged("IsBold");
			}
		}
	}
}