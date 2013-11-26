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
		public void GKWriteConfiguration(XDevice device)
		{
			GkDescriptorsWriter.WriteConfig(device);
			AddMessage("Запись конфигурации в прибор", "Сброс", XStateClass.Info, device);
		}

		public OperationResult<XDeviceConfiguration> GKReadConfiguration(XDevice device)
		{
			AddMessage("Чтение конфигурации из прибора", "", XStateClass.Info, device);
			var descriptorReader = device.Driver.IsKauOrRSR2Kau ? (DescriptorReaderBase)new KauDescriptorsReaderBase() : new GkDescriptorsReaderBase();
			descriptorReader.ReadConfiguration(device);
			return new OperationResult<XDeviceConfiguration>() { HasError = !string.IsNullOrEmpty(descriptorReader.ParsingError), Error = descriptorReader.ParsingError, Result = descriptorReader.DeviceConfiguration };
		}

		public void GKUpdateFirmware(XDevice device, string fileName)
		{
			AddMessage("Обновление ПО прибора", "", XStateClass.Info, device);
			FirmwareUpdateHelper.Update(device, fileName);
		}

		public bool GKSyncronyseTime(XDevice device)
		{
			AddMessage("Синхронизация времени", "", XStateClass.Info, device);
			return DeviceBytesHelper.WriteDateTime(device);
		}

		public string GKGetDeviceInfo(XDevice device)
		{
			AddMessage("Запрос информации об устройсве", "", XStateClass.Info, device);
			return DeviceBytesHelper.GetDeviceInfo(device);
		}

		public OperationResult<int> GKGetJournalItemsCount(XDevice device)
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

		public OperationResult<JournalItem> GKReadJournalItem(XDevice device, int no)
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

		public OperationResult<bool> GKSetSingleParameter(XDevice device)
		{
			var error = ParametersHelper.SetSingleParameter(device);
			return new OperationResult<bool>() { HasError = !string.IsNullOrEmpty(error), Error = error, Result = true };
		}

		public OperationResult<bool> GKGetSingleParameter(XDevice device)
		{
			var error = ParametersHelper.GetSingleParameter(device);
			return new OperationResult<bool>() { HasError = !string.IsNullOrEmpty(error), Error = error, Result = true };
		}

		public void GKExecuteDeviceCommand(XDevice device, XStateBit stateType)
		{
			Watcher.SendControlCommand(device, stateType);
			AddMessage("Команда оператора", stateType.ToDescription(), XStateClass.Info, device);
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

		public void GKResetFire1(Guid zoneUid)
		{
			var zone = GetXBase(zoneUid, XBaseObjectType.Zone);
			if (zone != null)
			{
				Watcher.SendControlCommand(zone, 0x02);
				AddMessage("Команда оператора", "Сброс", XStateClass.Info, zone);
			}
		}

		public void GKResetFire2(Guid zoneUid)
		{
			var zone = GetXBase(zoneUid, XBaseObjectType.Zone);
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
				//ObjectName = xBase.PresentationName,
				ObjectStateClass = XStateClass.Norm,
				GKObjectNo = (ushort)xBase.GKDescriptorNo,
				UserName = CurrentClientCredentials.UserName,
				SubsystemType = XSubsystemType.System
			};

			GKDBHelper.Add(journalItem);
			NotifyNewGKJournal(new List<JournalItem>() { journalItem });
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