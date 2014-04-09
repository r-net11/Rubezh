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
using System.Threading;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		string UserName
		{
			get { return CurrentClientCredentials.FriendlyUserName; }
		}

		public void CancelGKProgress(Guid progressCallbackUID, string userName)
		{
			GKProcessorManager.CancelGKProgress(progressCallbackUID, userName);
		}

		public OperationResult<bool> GKWriteConfiguration(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.BaseUID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKWriteConfiguration(device, UserName);
			}
			else
			{
				return new OperationResult<bool>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<XDeviceConfiguration> GKReadConfiguration(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.BaseUID == deviceUID);
			if (device != null)
			{
				DescriptorsManager.Create();
				return GKProcessorManager.GKReadConfiguration(device, UserName);
			}
			else
			{
				return new OperationResult<XDeviceConfiguration>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<XDeviceConfiguration> GKReadConfigurationFromGKFile(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.BaseUID == deviceUID);
			if (device != null)
			{
				DescriptorsManager.Create();
				return GKProcessorManager.GKReadConfigurationFromGKFile(device, UserName);
			}
			else
			{
				return new OperationResult<XDeviceConfiguration>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<bool> GKUpdateFirmware(Guid deviceUID, string fileName)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.BaseUID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKUpdateFirmware(device, fileName, UserName);
			}
			else
			{
				return new OperationResult<bool>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<bool> GKUpdateFirmwareFSCS(HexFileCollectionInfo hxcFileInfo, string userName, List<Guid> deviceUIDs)
		{
			var devices = new List<XDevice>();
			foreach (var deviceUID in deviceUIDs)
			{
				var device = XManager.Devices.FirstOrDefault(x => x.BaseUID == deviceUID);
				if (device == null)
				{
					return new OperationResult<bool>("Не найдено устройство в конфигурации");
				}
				devices.Add(device);
			}
			return GKProcessorManager.GKUpdateFirmwareFSCS(hxcFileInfo, userName, devices);
		}

		public OperationResult<bool> GKSyncronyseTime(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.BaseUID == deviceUID);
			if (device != null)
			{
				var result = GKProcessorManager.GKSyncronyseTime(device, UserName);
				if (result)
					return new OperationResult<bool>() { Result = true };
				else
					return new OperationResult<bool>("Устройство недоступно") { Result = false };
			}
			else
			{
				return new OperationResult<bool>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<string> GKGetDeviceInfo(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.BaseUID == deviceUID);
			if (device != null)
			{
				return new OperationResult<string>() { Result = GKProcessorManager.GKGetDeviceInfo(device, UserName) };
			}
			else
			{
				return new OperationResult<string>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<int> GKGetJournalItemsCount(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.BaseUID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKGetJournalItemsCount(device);
			}
			else
			{
				return new OperationResult<int>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<JournalItem> GKReadJournalItem(Guid deviceUID, int no)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.BaseUID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKReadJournalItem(device, no);
			}
			else
			{
				return new OperationResult<JournalItem>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<bool> GKSetSingleParameter(Guid objectUID, List<byte> parameterBytes)
		{
			XBase xBase = null;
			xBase = XManager.Devices.FirstOrDefault(x => x.BaseUID == objectUID);
			if (xBase == null)
			{
				xBase = XManager.Directions.FirstOrDefault(x => x.BaseUID == objectUID);
			}

			if (xBase != null)
			{
				return GKProcessorManager.GKSetSingleParameter(xBase, parameterBytes);
			}
			else
			{
				return new OperationResult<bool>("Не найден компонент в конфигурации");
			}
		}

		public OperationResult<List<XProperty>> GKGetSingleParameter(Guid objectUID)
		{
			XBase xBase = null;
			xBase = XManager.Devices.FirstOrDefault(x => x.BaseUID == objectUID);
			if (xBase == null)
			{
				xBase = XManager.Directions.FirstOrDefault(x => x.BaseUID == objectUID);
			}

			if (xBase != null)
			{
				return GKProcessorManager.GKGetSingleParameter(xBase);
			}
			else
			{
				return new OperationResult<List<XProperty>>("Не найден компонент в конфигурации");
			}
		}

		public OperationResult<List<byte>> GKGKHash(Guid gkDeviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.BaseUID == gkDeviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKGKHash(device);
			}
			else
			{
				return new OperationResult<List<byte>>("Не найдено устройство в конфигурации");
			}
		}

		public GKStates GKGetStates()
		{
			return GKProcessorManager.GKGetStates();
		}

		public void GKExecuteDeviceCommand(Guid deviceUID, XStateBit stateBit)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.BaseUID == deviceUID);
			if (device != null)
			{
				GKProcessorManager.GKExecuteDeviceCommand(device, stateBit, UserName);
			}
		}

		public void GKReset(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKReset(xBase, UserName);
			}
		}

		public void GKResetFire1(Guid zoneUID)
		{
			var zone = XManager.Zones.FirstOrDefault(x => x.BaseUID == zoneUID);
			if (zone != null)
			{
				GKProcessorManager.GKResetFire1(zone, UserName);
			}
		}

		public void GKResetFire2(Guid zoneUID)
		{
			var zone = XManager.Zones.FirstOrDefault(x => x.BaseUID == zoneUID);
			if (zone != null)
			{
				GKProcessorManager.GKResetFire2(zone, UserName);
			}
		}

		public void GKSetAutomaticRegime(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKSetAutomaticRegime(xBase, UserName);
			}
		}

		public void GKSetManualRegime(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKSetManualRegime(xBase, UserName);
			}
		}

		public void GKSetIgnoreRegime(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKSetIgnoreRegime(xBase, UserName);
			}
		}

		public void GKTurnOn(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOn(xBase, UserName);
			}
		}

		public void GKTurnOnNow(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOnNow(xBase, UserName);
			}
		}

		public void GKTurnOff(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOff(xBase, UserName);
			}
		}

		public void GKTurnOffNow(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOffNow(xBase, UserName);
			}
		}

		public void GKStop(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKStop(xBase, UserName);
			}
		}

		XBase GetXBase(Guid uid, XBaseObjectType objectType)
		{
			switch (objectType)
			{
				case XBaseObjectType.Deivce:
					return XManager.Devices.FirstOrDefault(x => x.BaseUID == uid);
				case XBaseObjectType.Direction:
					return XManager.Directions.FirstOrDefault(x => x.BaseUID == uid);
				case XBaseObjectType.Zone:
					return XManager.Zones.FirstOrDefault(x => x.BaseUID == uid);
				case XBaseObjectType.PumpStation:
					return XManager.PumpStations.FirstOrDefault(x => x.BaseUID == uid);
				case XBaseObjectType.MPT:
					return XManager.MPTs.FirstOrDefault(x => x.BaseUID == uid);
				case XBaseObjectType.Pim:
					return XManager.AutoGeneratedPims.FirstOrDefault(x => x.BaseUID == uid);
				case XBaseObjectType.Delay:
					return XManager.AutoGeneratedDelays.FirstOrDefault(x => x.BaseUID == uid);
			}
			return null;
		}

		public void GKStartMeasureMonitoring(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.BaseUID == deviceUID);
			if (device != null)
			{
				GKProcessorManager.GKStartMeasureMonitoring(device);
			}
		}

		public void GKStopMeasureMonitoring(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.BaseUID == deviceUID);
			if (device != null)
			{
				GKProcessorManager.GKStopMeasureMonitoring(device);
			}
		}

		public void AddJournalItem(JournalItem journalItem)
		{
			GKDBHelper.Add(journalItem);
			var gkCallbackResult = new GKCallbackResult();
			gkCallbackResult.JournalItems.Add(journalItem);
			NotifyGKObjectStateChanged(gkCallbackResult);
		}

		public List<JournalItem> GetGKTopLastJournalItems(int count)
		{
			return GKDBHelper.GetGKTopLastJournalItems(count);
		}

		public void BeginGetGKFilteredArchive(XArchiveFilter archiveFilter, Guid archivePortionUID)
		{
			if (CurrentThread != null)
			{
				GKDBHelper.IsAbort = true;
				CurrentThread.Join(TimeSpan.FromMinutes(1));
				CurrentThread = null;
			}
			GKDBHelper.IsAbort = false;
			var thread = new Thread(new ThreadStart((new Action(() =>
			{
				GKDBHelper.ArchivePortionReady -= DatabaseHelper_ArchivePortionReady;
				GKDBHelper.ArchivePortionReady += DatabaseHelper_ArchivePortionReady;
				GKDBHelper.BeginGetGKFilteredArchive(archiveFilter, archivePortionUID, false);

			}))));
			thread.Name = "GK GetFilteredArchive";
			thread.IsBackground = true;
			CurrentThread = thread;
			thread.Start();
		}

		void DatabaseHelper_ArchivePortionReady(List<JournalItem> journalItems, Guid archivePortionUID)
		{
			FiresecService.NotifyGKArchiveCompleted(journalItems, archivePortionUID);
		}

		public List<string> GetDistinctGKJournalNames()
		{
			return GKDBHelper.EventNames;
		}

		public List<string> GetDistinctGKJournalDescriptions()
		{
			return GKDBHelper.EventDescriptions;
		}
	}
}