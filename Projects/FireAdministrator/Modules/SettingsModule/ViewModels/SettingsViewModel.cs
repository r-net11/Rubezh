using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using SettingsModule.Views;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.IO;
using System.Windows.Documents;

namespace SettingsModule.ViewModels
{
	public class SettingsViewModel : ViewPartViewModel
	{
		public SettingsViewModel()
		{
			ShowDriversCommand = new RelayCommand(OnShowDrivers);
			ShowTreeCommand = new RelayCommand(OnShowTree);
			ConvertConfigurationCommand = new RelayCommand(OnConvertConfiguration);
			ConvertJournalCommand = new RelayCommand(OnConvertJournal);

			Test1Command = new RelayCommand(OnTest1);
			Test2Command = new RelayCommand(OnTest2);
		}

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
			}
		}

		public bool IsDebug
		{
			get { return ServiceFactory.AppSettings.IsDebug; }
		}

		public RelayCommand ShowDriversCommand { get; private set; }
		void OnShowDrivers()
		{
			var driversView = new DriversView();
			driversView.ShowDialog();
		}

		public RelayCommand ShowTreeCommand { get; private set; }
		void OnShowTree()
		{
			var devicesTreeViewModel = new DevicesTreeViewModel();
			DialogService.ShowModalWindow(devicesTreeViewModel);
		}

		public RelayCommand Test1Command { get; private set; }
		void OnTest1()
		{
			var stringBuilder = new StringBuilder();

			foreach (var driver in FiresecManager.Drivers)
			{
				foreach (var state in driver.States)
				{
					if (state.IsManualReset)
					{
						stringBuilder.AppendLine(driver.Name + " - " + state.Name + " - IsManualReset");
					}
				}
			}

			foreach (var driver in FiresecManager.Drivers)
			{
				foreach (var state in driver.States)
				{
					if (state.CanResetOnPanel)
					{
						stringBuilder.AppendLine(driver.Name + " - " + state.Name + " - CanResetOnPanel");
					}
				}
			}

			foreach (var driver in FiresecManager.Drivers)
			{
				foreach (var state in driver.States)
				{
					if (state.IsAutomatic && state.Name.Contains("AutoOff")) 
					{
						stringBuilder.AppendLine(driver.Name + " - " + state.Name + " - Automatic AutoOff");
					}
				}
			}

			foreach (var driver in FiresecManager.Drivers)
			{
				foreach (var state in driver.States)
				{
					if (state.IsAutomatic)
					{
						stringBuilder.AppendLine(driver.Name + " - " + state.Name + " - " + state.Code + " - Automatic");
					}
				}
			}
			Text = stringBuilder.ToString();
		}

		public RelayCommand Test2Command { get; private set; }
		void OnTest2()
		{
			string rtfString = "{\\rtf1\\ansi\\ansicpg1251\\deff0\\deflang1049\\fs20{\\fonttbl{\\f0\\fnil\\fprq2\\fcharset204 Arial;}\n{\\f99\\froman\\fcharset0\\fprq2{\\*\\panose 02020603050405020304}Arial;}}\n{\\colortbl ;\\red0\\green0\\blue0;\\red51\\green102\\blue255;}\n\\paperw11907\\paperh16839\\margl0\\margr0\\margt0\\margb0\n\\pard\\plain\\sb0\\ql\\fs20\\lang1049 \\pard\\plain \\fi-180\\li360 \\fs20\\lang1049\\bullet\\tab Устройство: ИП-64 1.10\n\\par\\pard\\sb0\\fs20\\lang1049 \\pard\\plain \\fi-180\\li360 \\fs20\\lang1049\\bullet\\tab Зона: 2\n\\par\\pard\\sb0\\fs20\\lang1049 \\ql \n\\par\\pard\\sb0\\fs20\\lang1049 \\ql {\\b Отладка (контекст): } \n\\par\\pard\\sb0\\fs20\\lang1049 \\ql 00 00 61 0A 00 00 02 00 01 95 \n\\par\\pard\\sb0\\fs20\\lang1049 \\ql 00 00 00 00 06 00 00 00 00 00 00 00 00 00 00 00 FB \\pard}";

			var richTextBox = new RichTextBox();
			MemoryStream memoryStream = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(rtfString));
			richTextBox.Selection.Load(memoryStream, DataFormats.Rtf);

			TextRange textrange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
			string plainText = textrange.Text;

			Text = plainText;
		}

		public RelayCommand ConvertConfigurationCommand { get; private set; }
		void OnConvertConfiguration()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите конвертировать конфигурацию?") == MessageBoxResult.Yes)
			{
				WaitHelper.Execute(() =>
				{
					FiresecManager.FiresecService.ConvertConfiguration();
					FiresecManager.GetConfiguration(false);
				});
				ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
			}
		}

		public RelayCommand ConvertJournalCommand { get; private set; }
		void OnConvertJournal()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите конвертировать журнал событий?") == MessageBoxResult.Yes)
			{
				WaitHelper.Execute(() =>
				{
					FiresecManager.FiresecService.ConvertJournal();
				});
			}
		}
	}
}