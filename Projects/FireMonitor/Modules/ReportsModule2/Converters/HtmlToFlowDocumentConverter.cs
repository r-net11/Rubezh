using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Documents;
using ReportsModule2.HTMLConverter;
using System.IO;
using System.Windows;

namespace ReportsModule2.Converters
{
	public class HtmlToFlowDocumentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter,
							  System.Globalization.CultureInfo culture)
		{
			if (value != null)
			{
				FlowDocument flowDocument = new FlowDocument();
				string xaml = HtmlToXamlConverter.ConvertHtmlToXaml(value.ToString(), false);

				using (MemoryStream stream = new MemoryStream((new ASCIIEncoding()).GetBytes(xaml)))
				{
					TextRange text = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
					text.Load(stream, DataFormats.Xaml);
				}

				return flowDocument;
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter,
								  System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
