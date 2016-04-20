using Infrastructure.Common.Windows.TreeList;
using Infrastructure.Common.Windows.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class ConsumerViewModel : TreeNodeViewModel<ConsumerViewModel>
	{
		public ConsumerViewModel(Consumer consumer)
		{
			Consumer = consumer;
		}

		Consumer _consumer;
		public Consumer Consumer
		{
			get { return _consumer; }
			set
			{
				_consumer = value;
				OnPropertyChanged(() => Consumer);
			}
		}

		public string ImageSource 
		{ 
			get
			{
				return _consumer != null && _consumer.IsFolder ? "/Controls;component/Images/CFolder.png" : "/Controls;component/Images/AccessTemplate.png";
			}
		}

		ConsumerDetailsViewModel _consumerDetails;
		public ConsumerDetailsViewModel ConsumerDetails
		{
			get
			{
				if (_consumerDetails == null && Consumer != null && !Consumer.IsFolder)
				{
					var consumer = DbCache.GetConsumer(Consumer.UID);
					if (consumer == null)
						return null;
					_consumerDetails = new ConsumerDetailsViewModel(consumer, true);
				}
				return _consumerDetails;
			}
		}
		public ConsumerDetailsViewModel GetConsumerDetails()
		{
			return _consumerDetails;
		}

		ConsumersFolderDetailsViewModel _consumersFolderDetails;
		public ConsumersFolderDetailsViewModel ConsumersFolderDetails
		{
			get
			{
				if (_consumersFolderDetails == null && Consumer != null && Consumer.IsFolder)
				{
					var consumer = DbCache.GetConsumer(Consumer.UID);
					if (consumer == null)
						return null;
					_consumersFolderDetails = new ConsumersFolderDetailsViewModel(consumer, true);
				}
				return _consumersFolderDetails;
			}
		}
		public ConsumersFolderDetailsViewModel GetConsumersFolderDetails()
		{
			return _consumersFolderDetails;
		}

		public void Update(Consumer consumer)
		{
			Consumer = consumer;
			if (_consumerDetails != null)
				_consumerDetails.Update(consumer);
			if (_consumersFolderDetails != null)
				_consumersFolderDetails.Update(consumer);
		}

		public Consumer GetConsumer()
		{
			if (_consumerDetails != null)
				return _consumerDetails.GetConsumer();
			if (_consumersFolderDetails != null)
				return _consumersFolderDetails.GetConsumer();
			return null;
		}
	}
}