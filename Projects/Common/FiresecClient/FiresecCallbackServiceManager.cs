using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Common;

namespace FiresecClient
{
	public static class FiresecCallbackServiceManager
	{
		static ServiceHost _serviceHost;
		static string _clientCallbackAddress;

		public static void Open(string clientCallbackAddress)
		{
			_clientCallbackAddress = clientCallbackAddress;
			var thread = new Thread(OnOpen);
			thread.Start();
		}

		static void OnOpen()
		{
			Close();
			_serviceHost = new ServiceHost(typeof(FiresecCallbackService));

			Binding binding = BindingHelper.CreateBindingFromAddress(_clientCallbackAddress);

			string machineName = MachineNameHelper.GetMachineName();
			_clientCallbackAddress = _clientCallbackAddress.Replace("localhost", machineName);
			_clientCallbackAddress = _clientCallbackAddress.Replace("localhost", "127.0.0.1");
			_serviceHost.AddServiceEndpoint("FiresecAPI.IFiresecCallbackService", binding, new Uri(_clientCallbackAddress));

			_serviceHost.Open();
		}

		public static void Close()
		{
			if (_serviceHost != null && _serviceHost.State != CommunicationState.Closed && _serviceHost.State != CommunicationState.Closing)
				_serviceHost.Close();
		}
	}
}