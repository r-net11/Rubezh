using System;
using System.ServiceModel;
using Common;
using RubezhAPI;

namespace RubezhClient
{
	public class ImitatorServiceFactory
	{
		readonly ChannelFactory<IImitatorService> _channelFactory;

		public ImitatorServiceFactory(string imitatorAddress)
		{
			var binding = BindingHelper.CreateBindingFromAddress(imitatorAddress);
			var endpointAddress = new EndpointAddress(new Uri(imitatorAddress));
			_channelFactory = new ChannelFactory<IImitatorService>(binding, endpointAddress);
			_channelFactory.Open();
		}

		public IImitatorService Create()
		{
			try
			{
				IImitatorService imitatorService = _channelFactory.CreateChannel();
				return imitatorService;
			}
			catch (Exception e)
			{
				Logger.Error(e, "ImitatorServiceFactory.Create");
			}
			return null;
		}
	}
}
