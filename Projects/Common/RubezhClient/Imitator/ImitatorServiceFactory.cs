using System;
using System.ServiceModel;
using Common;
using RubezhAPI;

namespace RubezhClient
{
	public class ImitatorServiceFactory
	{
		readonly ChannelFactory<IImitatorService> _channelFactory;

		public ImitatorServiceFactory()
		{
			var binding = BindingHelper.CreateBindingFromAddress("net.pipe://127.0.0.1/GKImitator/");
			var endpointAddress = new EndpointAddress(new Uri("net.pipe://127.0.0.1/GKImitator/"));
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
