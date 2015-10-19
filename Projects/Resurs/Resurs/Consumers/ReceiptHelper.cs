using Resurs.Reports.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Resurs.Consumers
{
	public static class ReceiptHelper
	{
		static DataContractSerializer _serializer = new DataContractSerializer(typeof(ReceiptTemplate));
		public static List<ReceiptTemplate> GetAllTemplate()
		{
			var templates = new List<ReceiptTemplate>();
			templates.Add(GetDefaultTemplate());
			return templates;
		}
		public static ReceiptTemplate GetDefaultTemplate()
		{
			var template = new ReceiptTemplate() { Name = "По умолчанию" };
			return template;
		}
		public static bool SaveReceipt(ReceiptTemplate receipt )
		{
			using (var ms = new MemoryStream())
			{
				_serializer.WriteObject(ms, receipt);
			}
			return true;
		}
	}
}