using System.Windows;
using System.Windows.Controls;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class DeviceJournalViewModel : DialogViewModel
	{
		public DeviceJournalViewModel(string htmlJournal)
		{
			Title = "Журнал событий устройства";
			HtmlString = "<head>\n<meta http-equiv='Content-Type' content='text/html;charset=UTF-8'>\n</head>" + htmlJournal;
			OnPropertyChanged("HtmlString");
		}

		string _htmlString;
		public string HtmlString 
		{
			get { return _htmlString; }
			set
			{
				_htmlString = value;
				OnPropertyChanged("HtmlString");
			}
		}
	}

	public class BrowserBehavior
	{
		public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
				"Html",
				typeof(string),
				typeof(BrowserBehavior),
				new FrameworkPropertyMetadata(OnHtmlChanged));

		[AttachedPropertyBrowsableForType(typeof(WebBrowser))]
		public static string GetHtml(WebBrowser webBrowser)
		{
			return (string)webBrowser.GetValue(HtmlProperty);
		}

		public static void SetHtml(WebBrowser webBrowser, string value)
		{
			webBrowser.SetValue(HtmlProperty, value);
		}

		static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			WebBrowser webBrowser = dependencyObject as WebBrowser;
			if (webBrowser != null)
				webBrowser.NavigateToString(e.NewValue as string);
		}
	}
}