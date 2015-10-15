using Resurs.Reports.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.Consumers
{
	public static class ReceiptHelper
	{
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
	}
}