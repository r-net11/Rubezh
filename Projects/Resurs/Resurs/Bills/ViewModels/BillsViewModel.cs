using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class BillsViewModel : BaseViewModel
	{
		public BillsViewModel(List<Bill> bills, bool isReadOnly)
		{
			IsReadOnly = isReadOnly;
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<BillViewModel>(OnRemove);
			Update(bills);
		}

		bool _isReadOnly;
		public bool IsReadOnly
		{
			get { return _isReadOnly; }
			set
			{
				_isReadOnly = value;
				OnPropertyChanged(() => IsReadOnly);
			}
		}

		ObservableCollection<BillViewModel> _bills;
		public ObservableCollection<BillViewModel> Bills
		{
			get { return _bills; }
			set
			{
				_bills = value;
				OnPropertyChanged(() => Bills);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			Bills.Add(new BillViewModel(new Bill(), this, IsReadOnly));
		}

		public RelayCommand<BillViewModel> RemoveCommand { get; private set; }
		void OnRemove(BillViewModel bill)
		{
			Bills.Remove(bill);
		}

		public void Update(List<Bill> bills)
		{
			Bills = new ObservableCollection<BillViewModel>(bills.Select(x => new BillViewModel(x, this, IsReadOnly)));
		}

		public List<Bill> GetBills()
		{
			return Bills.Select(x => x.GetBill()).ToList();
		}
	}
}
