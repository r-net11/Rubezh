using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;
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
					var consumer = DBCash.GetConsumer(Consumer.UID, true);
					if (consumer == null)
						return null;
					_consumerDetails = new ConsumerDetailsViewModel(consumer, false, true);
				}
				return _consumerDetails;
			}
		}

		ConsumersFolderDetailsViewModel _consumersFolderDetails;
		public ConsumersFolderDetailsViewModel ConsumersFolderDetails
		{
			get
			{
				if (_consumersFolderDetails == null && Consumer != null && Consumer.IsFolder)
				{
					var consumer = DBCash.GetConsumer(Consumer.UID, true);
					if (consumer == null)
						return null;
					_consumersFolderDetails = new ConsumersFolderDetailsViewModel(consumer, false, true);
				}
				return _consumersFolderDetails;
			}
		}

		public void Update(Consumer consumer)
		{
			Consumer = consumer;
			if (_consumerDetails != null)
				_consumerDetails.Consumer = consumer;
			if (_consumersFolderDetails != null)
				_consumersFolderDetails.Consumer = consumer;
		}
	}
}