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
	public class ConsumerListViewModel : BaseViewModel
	{
		public ConsumerListViewModel(Guid? exceptConsumerUid = null, bool isOnlyFolders = false)
			: this( exceptConsumerUid.HasValue ? new List<Guid> { exceptConsumerUid.Value } : null, isOnlyFolders)
		{
		}
		public ConsumerListViewModel(IList<Guid> exceptConsumerUids, bool isOnlyFolders = false)
		{
			ItemActivatedCommand = new RelayCommand(OnItemActivatedCommand);
			BuildTree(exceptConsumerUids, isOnlyFolders);
			if (RootConsumer != null)
			{
				SelectedConsumer = RootConsumer;
				SetIsExpanded(RootConsumer, true);
			}

			OnPropertyChanged(() => RootConsumers);
		}

		public event Action<ConsumerViewModel> OnItemActivated;
		public event Action<ConsumerViewModel> OnSelectedConsumerChanged;

		ConsumerViewModel _selectedConsumer;
		public ConsumerViewModel SelectedConsumer
		{
			get { return _selectedConsumer; }
			set
			{
				_selectedConsumer = value;
				OnPropertyChanged(() => SelectedConsumer);
				if (OnSelectedConsumerChanged != null)
					OnSelectedConsumerChanged(value);
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

		public RelayCommand ItemActivatedCommand { get; private set; }
		void OnItemActivatedCommand()
		{
			if (OnItemActivated != null)
				OnItemActivated(SelectedConsumer);
		}

		void SetIsExpanded(ConsumerViewModel consumer, bool isExpanded)
		{
			consumer.IsExpanded = isExpanded;
			foreach (var child in consumer.Children)
				SetIsExpanded(child, isExpanded);
		}

		void BuildTree(IList<Guid> exceptConsumerUids, bool isOnlyFolders)
		{
			RootConsumer = AddConsumerInternal(DbCache.RootConsumer, null, exceptConsumerUids, isOnlyFolders);
		}

		private ConsumerViewModel AddConsumerInternal(Consumer consumer, ConsumerViewModel parentConsumerViewModel, IList<Guid> exceptConsumerUids, bool isOnlyFolders)
		{
			var consumerViewModel = new ConsumerViewModel(consumer);
			if (parentConsumerViewModel != null)
				parentConsumerViewModel.AddChild(consumerViewModel);

			foreach (var childConsumer in consumer.Children.Where(x => (!isOnlyFolders || x.IsFolder) && (exceptConsumerUids == null || !exceptConsumerUids.Contains(x.UID))))
				AddConsumerInternal(childConsumer, consumerViewModel, exceptConsumerUids, isOnlyFolders);
			return consumerViewModel;
		}
	}
}