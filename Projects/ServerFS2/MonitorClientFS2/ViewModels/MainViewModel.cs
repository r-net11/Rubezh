using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;
using System.Xml.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using MonitorClientFS2.ViewModels;
using ServerFS2;
using ServerFS2.DataBase;

namespace MonitorClientFS2
{
	public class MainViewModel : BaseViewModel
	{
		List<MonitoringDevice> devicesLastRecord;

		public MainViewModel()
		{
			StartMonitoringCommand = new RelayCommand(OnStartMonitoring);
			StopMonitoringCommand = new RelayCommand(OnStopMonitoring);
			TestCommand = new RelayCommand(OnTest);

			DevicesViewModel = new DevicesViewModel();

			devicesLastRecord = new List<MonitoringDevice>();
			JournalItems = new ObservableCollection<FSJournalItem>(DBJournalHelper.GetJournalItems(new Guid()));

			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.IsPanel)
				{
					try
					{
						devicesLastRecord.Add(new MonitoringDevice(device));
					}
					catch
					{
						Trace.Write("Ошибка при считывании последней записи");
					}
				}
			}
			//OnStartMonitoring();
		}

		public DevicesViewModel DevicesViewModel { get; private set; }

		void AddToJournalObservable(FSJournalItem journalItem)
		{
			Dispatcher.Invoke(new Action(() =>
			{
				JournalItems.Add(journalItem);
			}));
		}

		ObservableCollection<FSJournalItem> _journalItems;
		public ObservableCollection<FSJournalItem> JournalItems
		{
			get { return _journalItems; }
			set
			{
				_journalItems = value;
				OnPropertyChanged("JournalItems");
			}
		}

		public RelayCommand StartMonitoringCommand { get; private set; }
		void OnStartMonitoring()
		{
			foreach (var device in devicesLastRecord)
			{
				device.StartMonitoring();
			}
			Trace.WriteLine("Начинаю мониторинг");
		}

		public RelayCommand StopMonitoringCommand { get; private set; }
		void OnStopMonitoring()
		{
			foreach (var device in devicesLastRecord)
			{
				device.StopMonitoring();
			}
			Trace.WriteLine("Останавливаю мониторинг");
		}

		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
			string fileName = "DeviceLastRecords.xml";

			XDocument deviceLastRecordsXml = new XDocument();

			XElement root = new XElement("DevicesLastRecords");
			deviceLastRecordsXml.Add(root);

			XElement device = new XElement("Device");
			device.Add(new XAttribute("Name", "2OP"));
			device.Add(new XAttribute("LastRecord", "111"));
			device.Add(new XAttribute("LastSecRecord", "222"));
			deviceLastRecordsXml.Root.Add(device);

			device = new XElement("Device");
			device.Add(new XAttribute("Name", "4A"));
			device.Add(new XAttribute("LastRecord", "333"));
			deviceLastRecordsXml.Root.Add(device);

			deviceLastRecordsXml.Save(fileName);

			XDocument doc = XDocument.Load(fileName);

			foreach (XElement el in doc.Root.Elements("Device"))
			{
				if (el.Attribute("LastSecRecord") != null)
				{
					el.Attribute("LastSecRecord").Value = "555";
					Trace.WriteLine(el.Attribute("LastSecRecord").Value);
				}
			}

			device = new XElement("Device");
			device.Add(new XAttribute("Name", "2AМ"));
			device.Add(new XAttribute("LastRecord", "999"));
			doc.Root.Add(device);

			doc.Save(fileName);
		}
	}
}