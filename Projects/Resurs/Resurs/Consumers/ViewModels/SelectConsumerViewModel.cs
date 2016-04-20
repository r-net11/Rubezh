using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
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
	public class SelectConsumerViewModel : SaveCancelDialogViewModel
	{
		public SelectConsumerViewModel(string title, Guid? exceptConsumerUid = null, bool isOnlyFolders = false)
		{
			Title = title;
			ConsumerListViewModel = new ConsumerListViewModel(exceptConsumerUid, isOnlyFolders);
			ConsumerListViewModel.OnItemActivated += ConsumerListViewModel_OnItemActivated;
		}

		public ConsumerListViewModel ConsumerListViewModel { get; private set; }
		public ConsumerViewModel SelectedConsumer { get; private set; }

		void ConsumerListViewModel_OnItemActivated(ConsumerViewModel consumer)
		{
			SelectedConsumer = consumer;
			Close(true);
		}
	}
}