using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using Infrastructure.Common;
using Ionic.Zip;
using FiresecAPI;
using System.Text;
using FiresecAPI.Models;
using XFiresecAPI;
using System;
using GKProcessor;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public void GKWriteConfiguration(XDevice device)
		{
			GkDescriptorsWriter.WriteConfig(device);
			//FiresecManager.FiresecService.NotifyClientsOnConfigurationChanged();
		}

		public void GKSetNewConfiguration(XDeviceConfiguration deviceConfiguration)
		{
		}

		public void GKExecuteDeviceCommand(XDevice device, XStateBit stateType)
		{
			Watcher.SendControlCommand(device, stateType);
			AddMessage("Команда оператора", stateType.ToDescription(), XStateClass.Info, device);
		}

		public void GKReset(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.Reset);
			AddMessage("Команда оператора", "Сброс", XStateClass.Info, xBase);
		}

		public void GKResetFire1(XZone zone)
		{
			Watcher.SendControlCommand(zone, 0x02);
			AddMessage("Команда оператора", "Сброс", XStateClass.Info, zone);
		}

		public void GKResetFire2(XZone zone)
		{
			Watcher.SendControlCommand(zone, 0x03);
			AddMessage("Команда оператора", "Сброс", XStateClass.Info, zone);
		}

		public void GKSetAutomaticRegime(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Automatic);
			AddMessage("Команда оператора", "Перевод в автоматический режим", XStateClass.Info, xBase);
		}

		public void GKSetManualRegime(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Manual);
			AddMessage("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase);
		}

		public void GKSetIgnoreRegime(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Off);
			AddMessage("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase);
		}

		public void GKTurnOn(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOn_InManual);
			AddMessage("Команда оператора", "Включить", XStateClass.Info, xBase);
		}

		public void GKTurnOnNow(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOnNow_InManual);
			AddMessage("Команда оператора", "Включить немедленно", XStateClass.Info, xBase);
		}

		public void GKTurnOff(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOff_InManual);
			AddMessage("Команда оператора", "Выключить", XStateClass.Info, xBase);
		}

		public void GKTurnOffNow(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOffNow_InManual);
			AddMessage("Команда оператора", "Выключить немедленно", XStateClass.Info, xBase);
		}

		public void GKStop(XBase xBase)
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
				//UserName = FiresecManager.CurrentUser.Name,
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