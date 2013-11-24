using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.Models.Skud;
using GKProcessor;
using XFiresecAPI;

namespace FiresecClient
{
    public partial class SafeFiresecService
    {
		static bool IsGKAsAService = false;

		public void GKStart()
		{
			if (IsGKAsAService)
			{

			}
			else
			{
				WatcherManager.Start();
			}
		}

		public void GKStop()
		{
		}

		public void GKStartConfigurationReloading()
		{
			if (IsGKAsAService)
			{

			}
			else
			{
				WatcherManager.LastConfigurationReloadingTime = DateTime.Now;
				WatcherManager.IsConfigurationReloading = true;
			}
		}

		public void GKStopConfigurationReloading()
		{
			if (IsGKAsAService)
			{

			}
			else
			{
				WatcherManager.IsConfigurationReloading = false;
			}
		}

		public void GKWriteConfiguration(XDevice device)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKWriteConfiguration(device); }, "GKWriteConfiguration");
			}
			else
			{
				GkDescriptorsWriter.WriteConfig(device);
				FiresecManager.FiresecService.NotifyClientsOnConfigurationChanged();
				AddMessage("Запись конфигурации в прибор", "Сброс", XStateClass.Info, device);
			}
		}

		public OperationResult<XDeviceConfiguration> GKReadConfiguration(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall<XDeviceConfiguration>(() => { return FiresecService.GKReadConfiguration(device); }, "GKReadConfiguration");
			}
			else
			{
				AddMessage("Чтение конфигурации из прибора", "", XStateClass.Info, device);
				var descriptorReader = device.Driver.IsKauOrRSR2Kau ? (DescriptorReaderBase)new KauDescriptorsReaderBase() : new GkDescriptorsReaderBase();
				descriptorReader.ReadConfiguration(device);
				return new OperationResult<XDeviceConfiguration>() { HasError = !string.IsNullOrEmpty(descriptorReader.ParsingError), Error = descriptorReader.ParsingError, Result = descriptorReader.DeviceConfiguration };
			}
		}

		public void GKUpdateFirmware(XDevice device, string fileName)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKUpdateFirmware(device, fileName); }, "GKUpdateFirmware");
			}
			else
			{
				AddMessage("Обновление ПО прибора", "", XStateClass.Info, device);
				FirmwareUpdateHelper.Update(device, fileName);
			}
		}

		public bool GKSyncronyseTime(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => { return FiresecService.GKSyncronyseTime(device); }, "GKSyncronyseTime");
			}
			else
			{
				AddMessage("Синхронизация времени", "", XStateClass.Info, device);
				return DeviceBytesHelper.WriteDateTime(device);
			}
		}

		public string GKGetDeviceInfo(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => { return FiresecService.GKGetDeviceInfo(device); }, "GKGetDeviceInfo");
			}
			else
			{
				AddMessage("Запрос информации об устройсве", "", XStateClass.Info, device);
				return DeviceBytesHelper.GetDeviceInfo(device);
			}
		}

		public OperationResult<int> GKGetJournalItemsCount(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => { return FiresecService.GKGetJournalItemsCount(device); }, "GKGetJournalItemsCount");
			}
			else
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
		}

		public OperationResult<JournalItem> GKReadJournalItem(XDevice device, int no)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => { return FiresecService.GKReadJournalItem(device, no); }, "GKReadJournalItem");
			}
			else
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
		}

		public OperationResult<bool> GKSetSingleParameter(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall<bool>(() => { return FiresecService.GKSetSingleParameter(device); }, "SetSingleParameter");
			}
			else
			{
				var error = ParametersHelper.SetSingleParameter(device);
				return new OperationResult<bool>() { HasError = !string.IsNullOrEmpty(error), Error = error, Result = true };
			}
		}

		public OperationResult<bool> GKGetSingleParameter(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall<bool>(() => { return FiresecService.GKGetSingleParameter(device); }, "GetSingleParameter");
			}
			else
			{
				var error = ParametersHelper.GetSingleParameter(device);
				return new OperationResult<bool>() { HasError = !string.IsNullOrEmpty(error), Error = error, Result = true };
			}
		}

		public void GKSetNewConfiguration(XDeviceConfiguration deviceConfiguration)
		{
			if (IsGKAsAService)
			{

			}
			else
			{

			}
		}

		public void GKExecuteDeviceCommand(XDevice device, XStateBit stateType)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKExecuteDeviceCommand(device, stateType); }, "GKExecuteDeviceCommand");
			}
			else
			{
				Watcher.SendControlCommand(device, stateType);
				AddMessage("Команда оператора", stateType.ToDescription(), XStateClass.Info, device);
			}
		}

		public void GKReset(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKReset(xBase); }, "GKReset");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.Reset);
				AddMessage("Команда оператора", "Сброс", XStateClass.Info, xBase);
			}
		}

		public void GKResetFire1(XZone zone)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKResetFire1(zone); }, "GKResetFire1");
			}
			else
			{
				Watcher.SendControlCommand(zone, 0x02);
				AddMessage("Команда оператора", "Сброс", XStateClass.Info, zone);
			}
		}

		public void GKResetFire2(XZone zone)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKResetFire2(zone); }, "GKResetFire2");
			}
			else
			{
				Watcher.SendControlCommand(zone, 0x03);
				AddMessage("Команда оператора", "Сброс", XStateClass.Info, zone);
			}
		}

		public void GKSetAutomaticRegime(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKSetAutomaticRegime(xBase); }, "GKSetAutomaticRegime");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Automatic);
				AddMessage("Команда оператора", "Перевод в автоматический режим", XStateClass.Info, xBase);
			}
		}

		public void GKSetManualRegime(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKSetManualRegime(xBase); }, "GKSetManualRegime");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Manual);
				AddMessage("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase);
			}
		}

		public void GKSetIgnoreRegime(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOn(xBase); }, "GKTurnOn");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Off);
				AddMessage("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase);
			}
		}

		public void GKTurnOn(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOn(xBase); }, "GKTurnOn");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.TurnOn_InManual);
				AddMessage("Команда оператора", "Включить", XStateClass.Info, xBase);
			}
		}

		public void GKTurnOnNow(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOnNow(xBase); }, "GKTurnOnNow");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.TurnOnNow_InManual);
				AddMessage("Команда оператора", "Включить немедленно", XStateClass.Info, xBase);
			}
		}

		public void GKTurnOff(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOff(xBase); }, "GKTurnOff");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.TurnOff_InManual);
				AddMessage("Команда оператора", "Выключить", XStateClass.Info, xBase);
			}
		}

		public void GKTurnOffNow(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOffNow(xBase); }, "GKTurnOffNow");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.TurnOffNow_InManual);
				AddMessage("Команда оператора", "Выключить немедленно", XStateClass.Info, xBase);
			}
		}

		public void GKStop(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKStop(xBase); }, "GKStop");
			}
			else
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
				UserName = FiresecManager.CurrentUser.Name,
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