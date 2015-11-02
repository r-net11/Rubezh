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
	public class FilterConsumersViewModel : BaseViewModel
	{
		public FilterConsumersViewModel(Filter filter)
		{
			BuildTree();
			AllConsumers.ForEach(x=> 
				{
					if (filter.ConsumerUIDs.Contains(x.Consumer.UID))
						x.IsChecked = true;
				});
			if (RootcConsumer != null)
			{
				RootcConsumer.IsExpanded = true;
				foreach (var child in RootcConsumer.Children)
					child.IsExpanded = true;
			}
		}

		public FilterConsumerViewModel RootcConsumer { get; set; }

		List<FilterConsumerViewModel> AllConsumers { get; set; }


		public FilterConsumerViewModel[] RootConsumers
		{
			get { return new[] { RootcConsumer }; }
		}

		void BuildTree()
		{
			RootcConsumer = AddConsumerInternal(DbCache.RootConsumer);
			FillAllConsumers();
		}

		private FilterConsumerViewModel AddConsumerInternal(Consumer consumer, FilterConsumerViewModel parentConsumerViewModel= null)
		{
			var consumerViewModel = new FilterConsumerViewModel(consumer);
			if (parentConsumerViewModel != null)
				parentConsumerViewModel.AddChild(consumerViewModel);

			foreach (var childConsumer in consumer.Children)
				AddConsumerInternal(childConsumer, consumerViewModel);
			return consumerViewModel;
		}

		public void FillAllConsumers()
		{
			AllConsumers = new List<FilterConsumerViewModel>();
			AddChildPlainConsumers(RootcConsumer);
		}
		void AddChildPlainConsumers(FilterConsumerViewModel parentViewModel)
		{
			AllConsumers.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainConsumers(childViewModel);
		}

		public List<Guid?> GetConsumerUIDs() 
		{
			List<Guid?> consumerUIDs = new List<Guid?>();
			foreach(var consumer in AllConsumers)
			{
				if(consumer.IsChecked)
				consumerUIDs.Add(consumer.Consumer.UID);
			}
			return consumerUIDs;
		}
	}
}