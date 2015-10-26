using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
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
		public Guid ConsumerUID { get; private set; }
		public DepositsViewModel(IEnumerable<Deposit> deposits, Guid consumerUID)
		{
			Title = "История пополнения баланса";
			ConsumerUID = consumerUID;
			Deposits = new ObservableCollection<DepositViewModel>(deposits.Select(x => new DepositViewModel(x)));

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
			var depositDetailsViewModel = new DepositDetailsViewModel(new Deposit { ConsumerUID = this.ConsumerUID, Moment = DateTime.Now }, true);
			if (DialogService.ShowModalWindow(depositDetailsViewModel))
			{
				var deposit = depositDetailsViewModel.GetDeposit();
				var depositViewModel = new DepositViewModel(deposit);
				Deposits.Add(depositViewModel);
				SelectedDeposit = depositViewModel;
				DBCash.SaveDeposit(deposit);
				//DBCash.AddJournalForUser(JournalType.AddDeposit, deposit);
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
				SelectedDeposit.Update(depositDetailsViewModel.GetDeposit());
				DBCash.SaveDeposit(SelectedDeposit.Deposit);
				//DBCash.AddJournalForUser(JournalType.AddDeposit, deposit);
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
			DBCash.DeleteDeposit(SelectedDeposit.Deposit);
			//DBCash.AddJournalForUser(JournalType.AddDeposit, deposit);
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
