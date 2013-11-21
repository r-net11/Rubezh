using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using XFiresecAPI;

namespace GKProcessor
{
	public class GKService : IGKService
	{
		public static string UserName;

		public GKService()
		{
		}

		public void Start()
		{
			WatcherManager.Start();
		}

		public void Stop()
		{
		}

		public void StartConfigurationReloading()
		{
			WatcherManager.LastConfigurationReloadingTime = DateTime.Now;
			WatcherManager.IsConfigurationReloading = true;
		}

		public void StopConfigurationReloading()
		{
			WatcherManager.IsConfigurationReloading = false;
		}

		public void SetNewConfiguration(XDeviceConfiguration deviceConfiguration)
		{
		}

		public void ExecuteDeviceCommand(XDevice device, XStateBit stateType)
		{
			Watcher.SendControlCommand(device, stateType);
			AddMessage("Команда оператора", stateType.ToDescription(), XStateClass.Info, device);
		}

		public void Reset(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.Reset);
			AddMessage("Команда оператора", "Сброс", XStateClass.Info, xBase);
		}

		public void ResetFire1(XZone zone)
		{
			Watcher.SendControlCommand(zone, 0x02);
			AddMessage("Команда оператора", "Сброс", XStateClass.Info, zone);
		}

		public void ResetFire2(XZone zone)
		{
			Watcher.SendControlCommand(zone, 0x03);
			AddMessage("Команда оператора", "Сброс", XStateClass.Info, zone);
		}

		public void SetAutomaticRegime(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Automatic);
			AddMessage("Команда оператора", "Перевод в автоматический режим", XStateClass.Info, xBase);
		}

		public void SetManualRegime(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Manual);
			AddMessage("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase);
		}

		public void SetIgnoreRegime(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Off);
			AddMessage("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase);
		}

		public void TurnOn(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOn_InManual);
			AddMessage("Команда оператора", "Включить", XStateClass.Info, xBase);
		}

		public void TurnOnNow(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOnNow_InManual);
			AddMessage("Команда оператора", "Включить немедленно", XStateClass.Info, xBase);
		}

		public void TurnOff(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOff_InManual);
			AddMessage("Команда оператора", "Выключить", XStateClass.Info, xBase);
		}

		public void TurnOffNow(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOffNow_InManual);
			AddMessage("Команда оператора", "Выключить немедленно", XStateClass.Info, xBase);
		}

		public void Stop(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.Stop_InManual);
			AddMessage("Команда оператора", "Остановка пуска", XStateClass.Info, xBase);
		}

		public void AddMessage(string message, string description, XStateClass stateClass, XBase xBase)
		{
			Guid uid = Guid.Empty;
			JournalItemType journalItemType = JournalItemType.System;
			if (xBase is XDevice)
			{
				uid = (xBase as XDevice).UID;
				journalItemType = JournalItemType.Device;
			}
			if (xBase is XZone)
			{
				uid = (xBase as XZone).UID;
				journalItemType = JournalItemType.Zone;
			}
			if (xBase is XDirection)
			{
				uid = (xBase as XDirection).UID;
				journalItemType = JournalItemType.Direction;
			}
			if (xBase is XDelay)
			{
				uid = (xBase as XDelay).UID;
				journalItemType = JournalItemType.Delay;
			}

			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalItemType = journalItemType,
				StateClass = stateClass,
				Name = message,
				Description = description,
				ObjectUID = uid,
				ObjectName = xBase.PresentationName,
				ObjectStateClass = XStateClass.Norm,
				GKObjectNo = (ushort)xBase.GKDescriptorNo,
				UserName = UserName,
				SubsystemType = XSubsystemType.System
			};

			GKDBHelper.Add(journalItem);
			OnNewJournalItems(journalItem);
		}

		public event Action<List<JournalItem>> NewJournalItems;
		void OnNewJournalItems(List<JournalItem> journalItems)
		{
			if (NewJournalItems != null)
				NewJournalItems(journalItems);
		}
		void OnNewJournalItems(JournalItem journalItem)
		{
			var journalItems = new List<JournalItem>() { journalItem };
			if (NewJournalItems != null)
				NewJournalItems(journalItems);
		}
	}
}