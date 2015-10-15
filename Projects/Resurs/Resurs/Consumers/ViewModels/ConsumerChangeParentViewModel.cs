using Infrastructure.Common.Windows.ViewModels;
using System;

namespace Resurs.ViewModels
{
	public class ConsumerChangeParentViewModel : SaveCancelDialogViewModel
	{
		public ConsumerChangeParentViewModel(Guid exceptConsumerUid)
		{
			Title = "Выбор группы для перемещения";
			ConsumerListViewModel = new ConsumerListViewModel(exceptConsumerUid, true);
			ConsumerListViewModel.OnItemActivated += ConsumerListViewModel_OnItemActivated;
		}

		public ConsumerListViewModel ConsumerListViewModel { get; private set; }
		public ConsumerViewModel SelectedConsumer {	get; private set; }

		void ConsumerListViewModel_OnItemActivated(ConsumerViewModel consumer)
		{
			SelectedConsumer = consumer;
			Close(true);
		}
	}
}