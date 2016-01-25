using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Common
{
	public static class BindingHelper
	{
		public static Binding CreateBindingFromAddress(string address)
		{
			Binding binding;
			if (address.StartsWith("net.pipe:"))
			{
				binding = CreateNetNamedPipeBinding();
			}
			else
			{
				binding = CreateNetTcpBinding();
			}
			return binding;
		}

		public static NetNamedPipeBinding CreateNetNamedPipeBinding()
		{
			var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			binding.OpenTimeout = TimeSpan.FromMinutes(10);
			binding.SendTimeout = TimeSpan.FromMinutes(10);
			binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
			binding.MaxReceivedMessageSize = Int32.MaxValue;
			binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
			binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
			binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;
			binding.Security.Mode = NetNamedPipeSecurityMode.None;
			binding.TransferMode = TransferMode.Streamed;
			return binding;
		}

		public static NetTcpBinding CreateNetTcpBinding()
		{
			var binding = new NetTcpBinding(SecurityMode.Message);
			binding.OpenTimeout = TimeSpan.FromMinutes(10);
			binding.SendTimeout = TimeSpan.FromMinutes(10);
			binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
			binding.MaxReceivedMessageSize = Int32.MaxValue;
			binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
			binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
			binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
			binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;
			binding.Security.Mode = SecurityMode.None;
			binding.TransferMode = TransferMode.Streamed;
			return binding;
		}

		public static WSHttpBinding CreateWSHttpBinding()
		{
			var binding = new WSHttpBinding(SecurityMode.None);
			binding.OpenTimeout = TimeSpan.FromMinutes(10);
			binding.SendTimeout = TimeSpan.FromMinutes(10);
			binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
			binding.MaxReceivedMessageSize = Int32.MaxValue;
			binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
			binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
			binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
			binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;
			binding.Security.Mode = SecurityMode.None;
			return binding;
		}
	}
}