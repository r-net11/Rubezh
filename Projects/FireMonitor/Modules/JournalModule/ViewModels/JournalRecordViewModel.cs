using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace JournalModule.ViewModels
{
	public class JournalRecordViewModel : BaseViewModel
	{
		public JournalRecord JournalRecord { get; private set; }
		readonly Device _device;

		public JournalRecordViewModel(JournalRecord journalRecord)
		{
			ShowPlanCommand = new RelayCommand(OnShowPlan, CanShowPlan);
			ShowTreeCommand = new RelayCommand(OnShowTree, CanShowTree);
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);

			JournalRecord = journalRecord;

			if (string.IsNullOrWhiteSpace(journalRecord.DeviceDatabaseId) == false)
				_device = FiresecManager.Devices.FirstOrDefault(x => x.DatabaseId == journalRecord.DeviceDatabaseId);
			else
				_device = FiresecManager.Devices.FirstOrDefault(x => x.DatabaseId == journalRecord.PanelDatabaseId);
		}

		public string Description
		{
			get
			{
				if (string.IsNullOrEmpty(JournalRecord.Detalization))
					return null;

				try
				{
					var memoryStream = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(JournalRecord.Detalization));
					var richTextBox = new RichTextBox();
					richTextBox.Selection.Load(memoryStream, DataFormats.Rtf);
					TextRange textrange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
					return textrange.Text;
				}
				catch (Exception e)
				{
					Logger.Error("JournalRecordViewModel.Description JournalRecord.Detalization = " + JournalRecord.Detalization);
					return null;
				}
			}
		}

		public RelayCommand ShowPlanCommand { get; private set; }
		void OnShowPlan()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(_device.UID);
		}

		bool CanShowPlan()
		{
			if (_device != null)
				return ExistsOnPlan();
			return false;
		}

		bool ExistsOnPlan()
		{
			foreach (var plan in FiresecManager.PlansConfiguration.Plans)
			{
				if (plan != null && plan.ElementDevices.IsNotNullOrEmpty())
				{
					var elementDevice = plan.ElementDevices.FirstOrDefault(x => x.DeviceUID == _device.UID);
					if (elementDevice != null)
						return true;
				}
			}
			return false;
		}

		public RelayCommand ShowTreeCommand { get; private set; }
		void OnShowTree()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(_device.UID);
		}

		bool CanShowTree()
		{
			return _device != null;
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			var zone = FiresecManager.Zones.FirstOrDefault(x => x.PresentationName == JournalRecord.ZoneName);
			ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(zone.No);
		}

		bool CanShowZone()
		{
			return string.IsNullOrEmpty(JournalRecord.ZoneName) == false;
		}
	}
}