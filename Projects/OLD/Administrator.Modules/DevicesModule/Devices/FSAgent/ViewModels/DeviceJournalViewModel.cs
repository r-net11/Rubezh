using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;

namespace DevicesModule.ViewModels
{
	public class DeviceJournalViewModel : DialogViewModel
	{
		public DeviceJournalViewModel(string htmlJournal)
		{
			Title = "Журнал событий устройства";
			SaveCommand = new RelayCommand(OnSave);
			HtmlString = "<head>\n<meta http-equiv='Content-Type' content='text/html;charset=UTF-8'>\n</head>" + htmlJournal;
			OnPropertyChanged(() => HtmlString);
		}

		string _htmlString;
		public string HtmlString 
		{
			get { return _htmlString; }
			set
			{
				_htmlString = value;
				OnPropertyChanged(() => HtmlString);
			}
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			try
			{
				var saveDialog = new SaveFileDialog()
				{
					Filter = "html files|*.html",
					DefaultExt = "html files|*.html"
				};

				if (saveDialog.ShowDialog().Value)
				{
					WaitHelper.Execute(() =>
					{
						using (var streamWriter = new StreamWriter(saveDialog.FileName))
						{
							streamWriter.Write(HtmlString);
						}
					});
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.SaveToFile");
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
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