using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Resurs.Processor;
using Resurs.Reports.Templates;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class SelectBillViewModel : SaveCancelDialogViewModel
	{
		public SelectBillViewModel()
		{
			Title = "Выбор счета";
			ConsumerListViewModel = new ConsumerListViewModel();
			ConsumerListViewModel.OnSelectedConsumerChanged += ConsumerListViewModel_OnSelectedConsumerChanged;
		}

		void ConsumerListViewModel_OnSelectedConsumerChanged(ConsumerViewModel consumer)
		{
			Bills = consumer != null && !consumer.Consumer.IsFolder ?
				new BillsViewModel(consumer.ConsumerDetails.BillsViewModel.GetBills(), true, true) :
				null;
		}

		public ConsumerListViewModel ConsumerListViewModel { get; private set; }

		BillsViewModel _bills;
		public BillsViewModel Bills
		{
			get { return _bills; }
			set
			{
				_bills = value;
				OnPropertyChanged(() => Bills);
			}
		}
	}
}