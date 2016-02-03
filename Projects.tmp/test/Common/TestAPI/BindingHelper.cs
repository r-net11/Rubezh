using System;
using System.ServiceModel;

namespace TestAPI
{
	public static class BindingHelper
	{
		public static NetTcpBinding CreateBinding()
		{
			var binding = new NetTcpBinding(SecurityMode.Message);
			binding.OpenTimeout = TimeSpan.FromMinutes(10);
			binding.SendTimeout = TimeSpan.FromMinutes(10);
			binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
			binding.MaxReceivedMessageSize = Int32.MaxValue;
			binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas();
			binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
			binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
			binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;
			binding.Security.Mode = SecurityMode.None;
			binding.TransferMode = TransferMode.Streamed;
			return binding;
		}
	}
}
