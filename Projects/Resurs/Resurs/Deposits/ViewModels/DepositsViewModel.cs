using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class DepositsViewModel : SaveCancelDialogViewModel
	{
		public Consumer Consumer { get; private set; }
		public DepositsViewModel(Consumer consumer)
		{
			Title = "История пополнения баланса";
			Consumer = consumer;
			Deposits = new ObservableCollection<DepositViewModel>(consumer.Deposits.Select(x => new DepositViewModel(x)));

			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
		}

		public ObservableCollection<DepositViewModel> Deposits { get; private set; }

		DepositViewModel _selectedDeposit;
		public DepositViewModel SelectedDeposit
		{
			get { return _selectedDeposit; }
			set
			{
				_selectedDeposit = value;
				OnPropertyChanged(() => SelectedDeposit);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var depositDetailsViewModel = new DepositDetailsViewModel(new Deposit 
			{ 
				Name = "ЛС: " + Consumer.Number,
				ConsumerUID = Consumer.UID, 
				Moment = DateTime.Now 
			}, true);
			if (DialogService.ShowModalWindow(depositDetailsViewModel))
			{
				var deposit = depositDetailsViewModel.GetDeposit();
				var depositViewModel = new DepositViewModel(deposit);
				Deposits.Add(depositViewModel);
				SelectedDeposit = depositViewModel;
				DbCache.SaveDeposit(deposit);
				DbCache.AddJournalForUser(JournalType.AddDeposit, deposit, string.Format("Сумма: {0} руб.", deposit.Amount));
			}
		}
		bool CanAdd()
		{
			return true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var depositDetailsViewModel = new DepositDetailsViewModel(SelectedDeposit.Deposit, false);
			if (DialogService.ShowModalWindow(depositDetailsViewModel))
			{
				var deposit = depositDetailsViewModel.GetDeposit();
				SelectedDeposit.Update(deposit);
				DbCache.SaveDeposit(SelectedDeposit.Deposit);
				DbCache.AddJournalForUser(JournalType.EditDeposit, deposit, string.Format("Сумма: {0} руб.", deposit.Amount));
			}
		}
		bool CanEdit()
		{
			return SelectedDeposit != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var index = Deposits.IndexOf(SelectedDeposit);
			DbCache.DeleteDeposit(SelectedDeposit.Deposit);
			DbCache.AddJournalForUser(JournalType.DeleteDeposit, SelectedDeposit.Deposit, string.Format("Сумма: {0} руб.", SelectedDeposit.Deposit.Amount));
			Deposits.Remove(SelectedDeposit);
			if (index >= Deposits.Count)
				index = Deposits.Count - 1;
			SelectedDeposit = Deposits.ElementAtOrDefault(index);
		}
		bool CanRemove()
		{
			return SelectedDeposit != null;
		}
	}
}
