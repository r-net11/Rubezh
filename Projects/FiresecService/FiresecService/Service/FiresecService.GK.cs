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
using FiresecClient;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public void AddJournalItem(JournalItem journalItem)
		{
			GKDBHelper.Add(journalItem);
			NotifyNewGKJournal(new List<JournalItem>() { journalItem });
		}

		public void GKWriteConfiguration(Guid deviceUID, bool writeFileToGK = false)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				GkDescriptorsWriter.WriteConfig(device);
				AddMessage("Запись конфигурации в прибор", "Сброс", XStateClass.Info, device);
			}
		}

		public OperationResult<XDeviceConfiguration> GKReadConfiguration(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddMessage("Чтение конфигурации из прибора", "", XStateClass.Info, device);
				var descriptorReader = device.Driver.IsKauOrRSR2Kau ? (DescriptorReaderBase)new KauDescriptorsReaderBase() : new GkDescriptorsReaderBase();
				descriptorReader.ReadConfiguration(device);
				return new OperationResult<XDeviceConfiguration>() { HasError = !string.IsNullOrEmpty(descriptorReader.Error), Error = descriptorReader.Error, Result = descriptorReader.DeviceConfiguration };
			}
			else
			{
				return new OperationResult<XDeviceConfiguration>("Не найдено устройство в конфигурации");
			}
		}

		public void GKUpdateFirmware(Guid deviceUID, string fileName)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddMessage("Обновление ПО прибора", "", XStateClass.Info, device);
				FirmwareUpdateHelper.Update(device, fileName);
			}
		}

		public bool GKSyncronyseTime(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddMessage("Синхронизация времени", "", XStateClass.Info, device);
				return DeviceBytesHelper.WriteDateTime(device);
			}
			else
			{
				return false;
			}
		}

		public string GKGetDeviceInfo(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddMessage("Запрос информации об устройсве", "", XStateClass.Info, device);
				return DeviceBytesHelper.GetDeviceInfo(device);
			}
			else
			{
				return null;
			}
		}

		public OperationResult<int> GKGetJournalItemsCount(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var sendResult = SendManager.Send(device, 0, 6, 64);
				if (sendResult.HasError)
				{
					return new OperationResult<int>("Ошибка связи с устройством");
				}
				var journalParser = new JournalParser(device, sendResult.Bytes);
				var result = journalParser.JournalItem.GKJournalRecordNo.Value;
				return new OperationResult<int>() { Result = result };
			}
			else
			{
				return new OperationResult<int>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<JournalItem> GKReadJournalItem(Guid deviceUID, int no)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var data = BitConverter.GetBytes(no).ToList();
				var sendResult = SendManager.Send(device, 4, 7, 64, data);
				if (sendResult.HasError)
				{
					return new OperationResult<JournalItem>("Ошибка связи с устройством");
				}
				var journalParser = new JournalParser(device, sendResult.Bytes);
				return new OperationResult<JournalItem>() { Result = journalParser.JournalItem };
			}
			else
			{
				return new OperationResult<JournalItem>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<bool> GKSetSingleParameter(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var error = ParametersHelper.SetSingleParameter(device);
				return new OperationResult<bool>() { HasError = !string.IsNullOrEmpty(error), Error = error, Result = true };
			}
			else
			{
				return new OperationResult<bool>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<bool> GKGetSingleParameter(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var error = ParametersHelper.GetSingleParameter(device);
				return new OperationResult<bool>() { HasError = !string.IsNullOrEmpty(error), Error = error, Result = true };
			}
			else
			{
				return new OperationResult<bool>("Не найдено устройство в конфигурации");
			}
		}

		public void GKExecuteDeviceCommand(Guid deviceUID, XStateBit stateBit)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				Watcher.SendControlCommand(device, stateBit);
				AddMessage("Команда оператора", stateBit.ToDescription(), XStateClass.Info, device);
			}
		}

		public void GKReset(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				Watcher.SendControlCommand(xBase, XStateBit.Reset);
				AddMessage("Команда оператора", "Сброс", XStateClass.Info, xBase);
			}
		}

		public void GKResetFire1(Guid zoneUID)
		{
			var zone = GetXBase(zoneUID, XBaseObjectType.Zone);
			if (zone != null)
			{
				Watcher.SendControlCommand(zone, 0x02);
				AddMessage("Команда оператора", "Сброс", XStateClass.Info, zone);
			}
		}

		public void GKResetFire2(Guid zoneUID)
		{
			var zone = GetXBase(zoneUID, XBaseObjectType.Zone);
			if (zone != null)
			{
				Watcher.SendControlCommand(zone, 0x03);
				AddMessage("Команда оператора", "Сброс", XStateClass.Info, zone);
			}
		}

		public void GKSetAutomaticRegime(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Automatic);
				AddMessage("Команда оператора", "Перевод в автоматический режим", XStateClass.Info, xBase);
			}
		}

		public void GKSetManualRegime(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Manual);
				AddMessage("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase);
			}
		}

		public void GKSetIgnoreRegime(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Off);
				AddMessage("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase);
			}
		}

		public void GKTurnOn(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				Watcher.SendControlCommand(xBase, XStateBit.TurnOn_InManual);
				AddMessage("Команда оператора", "Включить", XStateClass.Info, xBase);
			}
		}

		public void GKTurnOnNow(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				Watcher.SendControlCommand(xBase, XStateBit.TurnOnNow_InManual);
				AddMessage("Команда оператора", "Включить немедленно", XStateClass.Info, xBase);
			}
		}

		public void GKTurnOff(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				Watcher.SendControlCommand(xBase, XStateBit.TurnOff_InManual);
				AddMessage("Команда оператора", "Выключить", XStateClass.Info, xBase);
			}
		}

		public void GKTurnOffNow(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				Watcher.SendControlCommand(xBase, XStateBit.TurnOffNow_InManual);
				AddMessage("Команда оператора", "Выключить немедленно", XStateClass.Info, xBase);
			}
		}

		public void GKStop(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				Watcher.SendControlCommand(xBase, XStateBit.Stop_InManual);
				AddMessage("Команда оператора", "Остановка пуска", XStateClass.Info, xBase);
			}
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
				UserName = CurrentClientCredentials.UserName,
				SubsystemType = XSubsystemType.System
			};

			AddJournalItem(journalItem);
		}

		XBase GetXBase(Guid uid, XBaseObjectType objectType)
		{
			switch (objectType)
			{
				case XBaseObjectType.Deivce:
					return XManager.Devices.FirstOrDefault(x => x.UID == uid);
				case XBaseObjectType.Direction:
					return XManager.Directions.FirstOrDefault(x => x.UID == uid);
				case XBaseObjectType.Zone:
					return XManager.Zones.FirstOrDefault(x => x.UID == uid);
			}
			return null;
		}
	}
}