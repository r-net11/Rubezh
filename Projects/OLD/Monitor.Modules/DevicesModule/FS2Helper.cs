//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using FiresecClient;
//using FiresecAPI.Models;
//using FS2Api;
//using Infrastructure;
//using Infrastructure.Events;
//using System.Windows.Threading;
//using System.Windows;
//using Infrastructure.Common.Windows.Windows;

//namespace DevicesModule
//{
//	public static class FS2Helper
//	{
//		public static void Initialize()
//		{
//			FiresecManager.FS2ClientContract.DeviceStateChanged += new Action<List<DeviceState>>(OnDeviceStateChanged);
//			FiresecManager.FS2ClientContract.DeviceParametersChanged += new Action<List<DeviceState>>(OnDeviceParametersChanged);
//			FiresecManager.FS2ClientContract.ZoneStatesChanged += new Action<List<ZoneState>>(OnZoneStatesChanged);
//			FiresecManager.FS2ClientContract.NewJournalItems += new Action<List<FS2JournalItem>>(OnNewJournalItems);
//			FiresecManager.FS2ClientContract.NewArchiveJournalItems += new Action<List<FS2JournalItem>>(OnNewArchiveJournalItems);
//			FiresecManager.FS2ClientContract.Progress += new Action<FS2ProgressInfo>(OnProgress);
//			SafeFiresecService.NewJournalRecordEvent += new Action<JournalRecord>(OnNewServerJournalRecordEvent);
//		}

//		static void OnDeviceStateChanged(List<DeviceState> deviceStates)
//		{
//			if (ApplicationService.ApplicationWindow != null && ApplicationService.ApplicationWindow.Dispatcher != null)
//				ApplicationService.ApplicationWindow.Dispatcher.Invoke(new Action(() =>
//				{
//					foreach (var deviceState in deviceStates)
//					{
//						var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == deviceState.DeviceUID);
//						if (device != null)
//						{
//							FiresecManager.CopyDeviceStatesFromFS2Server(device, deviceState);
//							device.DeviceState.OnStateChanged();
//							ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Publish(null);
//						}
//					}
//				}));
//		}

//		static void OnDeviceParametersChanged(List<DeviceState> deviceStates)
//		{
//			if (ApplicationService.ApplicationWindow != null && ApplicationService.ApplicationWindow.Dispatcher != null)
//				ApplicationService.ApplicationWindow.Dispatcher.Invoke(new Action(() =>
//				{
//					foreach (var deviceState in deviceStates)
//					{
//						var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == deviceState.DeviceUID);
//						if (device != null)
//						{
//							FiresecManager.CopyDeviceStatesFromFS2Server(device, deviceState);
//							device.DeviceState.OnParametersChanged();
//							ServiceFactory.Events.GetEvent<DeviceParametersChangedEvent>().Publish(device.UID);
//						}
//					}
//				}));
//		}

//		static void OnZoneStatesChanged(List<ZoneState> zoneStates)
//		{
//			if (ApplicationService.ApplicationWindow != null && ApplicationService.ApplicationWindow.Dispatcher != null)
//				ApplicationService.ApplicationWindow.Dispatcher.Invoke(new Action(() =>
//				{
//					foreach (var zoneState in zoneStates)
//					{
//						var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == zoneState.ZoneUID);
//						if (zone != null)
//						{
//							zone.ZoneState.StateType = zoneState.StateType;
//							zone.ZoneState.OnStateChanged();
//						}
//					}
//				}));
//		}

//		static void OnNewJournalItems(List<FS2JournalItem> journalItems)
//		{
//			if (ApplicationService.ApplicationWindow != null && ApplicationService.ApplicationWindow.Dispatcher != null)
//				ApplicationService.ApplicationWindow.Dispatcher.BeginInvoke(new Action(() =>
//					{
//						ServiceFactory.Events.GetEvent<NewFS2JournalItemsEvent>().Publish(journalItems);
//					}));
//		}

//		static void OnNewArchiveJournalItems(List<FS2JournalItem> journalItems)
//		{
//			ServiceFactory.Events.GetEvent<GetFS2FilteredArchiveCompletedEvent>().Publish(journalItems);
//		}

//		static void OnProgress(FS2ProgressInfo progressInfo)
//		{
//			ServiceFactory.Events.GetEvent<FS2ProgressInfoEvent>().Publish(progressInfo);
//		}

//		static void OnNewServerJournalRecordEvent(JournalRecord journalRecord)
//		{
//			if (ApplicationService.ApplicationWindow != null && ApplicationService.ApplicationWindow.Dispatcher != null)
//				ApplicationService.ApplicationWindow.Dispatcher.BeginInvoke(new Action(() =>
//				{
//					var journalItems = new List<FS2JournalItem>();
//					var journalItem = new FS2JournalItem()
//					{
//						DeviceTime = journalRecord.DeviceTime,
//						SystemTime = journalRecord.SystemTime,
//						Description = journalRecord.Description,
//						Detalization = journalRecord.Detalization,
//						DeviceCategory = journalRecord.DeviceCategory,
//						StateType = journalRecord.StateType,
//						DeviceUID = journalRecord.DeviceDatabaseUID,
//						PanelUID = journalRecord.PanelDatabaseUID,
//						ZoneName = journalRecord.ZoneName,
//						UserName = journalRecord.User
//					};
//					journalItems.Add(journalItem);
//					ServiceFactory.Events.GetEvent<NewFS2JournalItemsEvent>().Publish(journalItems);
//				}));
//		}
//	}
//}