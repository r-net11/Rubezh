using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Resurs.Processor;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class ChangeParentViewModel : SaveCancelDialogViewModel
	{
		public ChangeParentViewModel(Guid exceptConsumerUid)
		{
			Title = "Выбор группы для перемещения";
			BuildTree(exceptConsumerUid);
			if (RootConsumer != null)
			{
				SelectedConsumer = RootConsumer;
				RootConsumer.IsExpanded = true;
				foreach (var child in RootConsumer.Children)
					child.IsExpanded = true;
			}

			foreach (var consumerViewModel in AllConsumers)
			{
				if (true)
					consumerViewModel.ExpandToThis();
			}

			OnPropertyChanged(() => RootConsumers);
		}

		ConsumerViewModel _selectedConsumer;
		public ConsumerViewModel SelectedConsumer
		{
			get { return _selectedConsumer; }
			set
			{
				_selectedConsumer = value;
				OnPropertyChanged(() => SelectedConsumer);
			}
		}

		ConsumerViewModel _rootConsumer;
		public ConsumerViewModel RootConsumer
		{
			get { return _rootConsumer; }
			private set
			{
				_rootConsumer = value;
				OnPropertyChanged(() => RootConsumer);
			}
		}

		public ConsumerViewModel[] RootConsumers
		{
			get { return new[] { RootConsumer }; }
		}

		void BuildTree(Guid exceptConsumerUid)
		{
			RootConsumer = AddConsumerInternal(DBCash.RootConsumer, null, exceptConsumerUid);
			FillAllConsumers();
		}

		private ConsumerViewModel AddConsumerInternal(Consumer consumer, ConsumerViewModel parentConsumerViewModel, Guid exceptConsumerUid)
		{
			var consumerViewModel = new ConsumerViewModel(consumer);
			if (parentConsumerViewModel != null)
				parentConsumerViewModel.AddChild(consumerViewModel);

			foreach (var childConsumer in consumer.Children.Where(x => x.IsFolder && x.UID != exceptConsumerUid))
				AddConsumerInternal(childConsumer, consumerViewModel, exceptConsumerUid);
			return consumerViewModel;
		}

		public List<ConsumerViewModel> AllConsumers;

		public void FillAllConsumers()
		{
			AllConsumers = new List<ConsumerViewModel>();
			AddChildPlainConsumers(RootConsumer);
		}

		void AddChildPlainConsumers(ConsumerViewModel parentViewModel)
		{
			AllConsumers.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainConsumers(childViewModel);
		}

		public void Select(Guid consumerUID)
		{
			if (consumerUID != Guid.Empty)
			{
				FillAllConsumers();
				var consumerViewModel = AllConsumers.FirstOrDefault(x => x.Consumer.UID == consumerUID);
				if (consumerViewModel != null)
					consumerViewModel.ExpandToThis();
				SelectedConsumer = consumerViewModel;
			}
		}
	}
}