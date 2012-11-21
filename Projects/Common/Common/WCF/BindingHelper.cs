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
			var binding = new NetNamedPipeBinding()
			{
				MaxBufferPoolSize = Int64.MaxValue,
				MaxConnections = 10,
				OpenTimeout = TimeSpan.FromMinutes(1),
				ReceiveTimeout = TimeSpan.FromMinutes(1),
				MaxBufferSize = Int32.MaxValue,
				MaxReceivedMessageSize = Int32.MaxValue,
			};
			binding.Security.Mode = NetNamedPipeSecurityMode.None;
			binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
			binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
			binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;
			return binding;
		}

		public static NetTcpBinding CreateNetTcpBinding()
		{
			var binding = new NetTcpBinding()
			{
				MaxBufferPoolSize = Int64.MaxValue,
				MaxConnections = 10,
				OpenTimeout = TimeSpan.FromMinutes(1),
				ListenBacklog = 10,
				ReceiveTimeout = TimeSpan.FromMinutes(1),
				MaxBufferSize = Int32.MaxValue,
				MaxReceivedMessageSize = Int32.MaxValue
			};
			binding.Security.Mode = SecurityMode.None;
			binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
			binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
			binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
			binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;
			return binding;
		}
	}
}