using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using System.Diagnostics;
using FS2Api;

namespace JournalModule.ViewModels
{
	public class JournalRecordViewModel : BaseViewModel
	{
		readonly Device Device;
		readonly Zone Zone;

		public JournalRecordViewModel(JournalRecord journalRecord)
		{
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowTreeCommand = new RelayCommand(OnShowTree, CanShowTree);
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);

			DeviceTime = journalRecord.DeviceTime;
			SystemTime = journalRecord.SystemTime;
			ZoneName = journalRecord.ZoneName;
			Description = journalRecord.Description;
			DeviceName = journalRecord.DeviceName;
			PanelName = journalRecord.PanelName;
			User = journalRecord.User;
			SubsystemType = journalRecord.SubsystemType;
			StateType = journalRecord.StateType;
			Detalization = GetDetalization(journalRecord.Detalization);

			if (journalRecord.DeviceDatabaseUID != Guid.Empty)
			{
				Device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == journalRecord.DeviceDatabaseUID);
			}
			else
			{
				Device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == journalRecord.PanelDatabaseUID);
			}

			Zone = FiresecManager.Zones.FirstOrDefault(x => x.FullPresentationName == journalRecord.ZoneName);
		}

		public JournalRecordViewModel(FS2JournalItem journalItem)
		{
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowTreeCommand = new RelayCommand(OnShowTree, CanShowTree);
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);

			DeviceTime = journalItem.DeviceTime;
			SystemTime = journalItem.SystemTime;
			ZoneName = journalItem.ZoneName;
			Description = journalItem.Description;
			DeviceName = journalItem.DeviceName;
			PanelName = journalItem.PanelName;
			User = journalItem.UserName;
			SubsystemType = journalItem.SubsystemType;
			StateType = journalItem.StateType;

			if (journalItem.DeviceUID != Guid.Empty)
				Device = FiresecManager.Devices.FirstOrDefault(x => x.UID == journalItem.DeviceUID);
			else
				Device = FiresecManager.Devices.FirstOrDefault(x => x.UID == journalItem.PanelUID);

			Zone = FiresecManager.Zones.FirstOrDefault(x => x.FullPresentationName == journalItem.ZoneName);
		}

		public DateTime DeviceTime { get; private set; }
		public DateTime SystemTime { get; private set; }
		public string ZoneName { get; private set; }
		public string Description { get; private set; }
		public string DeviceName { get; private set; }
		public string PanelName { get; private set; }
		public string User { get; private set; }
		public SubsystemType SubsystemType { get; private set; }
		public StateType StateType { get; private set; }
		public string Detalization { get; private set; }

		string GetDetalization(string detalization)
		{
			if (string.IsNullOrEmpty(detalization))
				return null;

			try
			{
				var memoryStream = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(detalization));
				var richTextBox = new RichTextBox();
				richTextBox.Selection.Load(memoryStream, DataFormats.Rtf);
				TextRange textrange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
				var result = textrange.Text;

				result = result.Replace("- ", "\r\n");

				//result = result.Replace("</li><li>", "\r\n");
				//result = result.Replace("<li>", "");
				//result = result.Replace("</li>", "");
				//if (result == "я")
				//    result = "";
				return result;
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalRecordViewModel.Description JournalRecord.Detalization = " + detalization);
				return null;
			}
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(Device.UID);
		}

		bool CanShowOnPlan()
		{
			if (Device != null)
				return ExistsOnPlan();
			return false;
		}

		bool ExistsOnPlan()
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				if (plan != null && plan.ElementDevices.IsNotNullOrEmpty())
				{
					var elementDevice = plan.ElementDevices.FirstOrDefault(x => x.DeviceUID == Device.UID);
					if (elementDevice != null)
						return true;
				}
			return false;
		}

		public RelayCommand ShowTreeCommand { get; private set; }
		void OnShowTree()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Device.UID);
		}
		bool CanShowTree()
		{
			return Device != null;
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(Zone.UID);
		}

		bool CanShowZone()
		{
			return Zone != null;
		}
	}
}