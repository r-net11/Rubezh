using System.Collections.Generic;
using Common;
using Infrastructure;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using System.Threading;
using System;
using System.Linq;
using System.Diagnostics;
using FiresecClient;
using Infrastructure.Common.Services;
using FiresecAPI;

namespace GKProcessor
{
	public partial class Watcher
	{
		bool IsDBMissmatchDuringMonitoring = false;
		string DBMissmatchDuringMonitoringReason = "";

		bool _isJournalAnyDBMissmatch = false;
		bool IsJournalAnyDBMissmatch
		{
			get { return _isJournalAnyDBMissmatch; }
			set
			{
				if (_isJournalAnyDBMissmatch != value)
				{
					_isJournalAnyDBMissmatch = value;
					var journalItem = new JournalItem()
					{
						GKIpAddress = XManager.GetIpAddress(GkDatabase.RootDevice),
						StateClass = XStateClass.Unknown,
						Name = value ? "База данных прибора не соответствует базе данных ПК" : "База данных прибора соответствует базе данных ПК"
					};
					AddJournalItem(journalItem);
				}
			}
		}

		bool GetAllStates(bool showProgress)
		{
			IsDBMissmatchDuringMonitoring = false;

			if (showProgress)
				GKProcessorManager.OnStartProgress("Опрос объектов ГК", "", GkDatabase.Descriptors.Count, false, GKProgressClientType.Monitor);
			foreach (var descriptor in GkDatabase.Descriptors)
			{
				LastUpdateTime = DateTime.Now;
				var result = GetState(descriptor.XBase);
				if (!IsConnected)
				{
					break;
				}
				if (showProgress)
					GKProcessorManager.OnDoProgress(descriptor.XBase.DescriptorPresentationName);

				WaitIfSuspending();
				if (IsStopping)
					return true;
			}
			if (showProgress)
				GKProcessorManager.OnStopProgress();

			foreach (var descriptor in GkDatabase.Descriptors)
			{
				descriptor.XBase.BaseState.IsGKMissmatch = IsDBMissmatchDuringMonitoring;
			}
			IsJournalAnyDBMissmatch = IsDBMissmatchDuringMonitoring;
			CheckTechnologicalRegime();
			NotifyAllObjectsStateChanged();
			return !IsDBMissmatchDuringMonitoring && IsConnected;
		}

		void NotifyAllObjectsStateChanged()
		{
			var gkDevice = XManager.Devices.FirstOrDefault(x => x.UID == GkDatabase.RootDevice.UID);
			foreach (var device in XManager.GetAllDeviceChildren(gkDevice))
			{
				OnObjectStateChanged(device);
			}
			foreach (var device in XManager.GetAllDeviceChildren(gkDevice))
			{
				if (device.Driver.IsGroupDevice || device.DriverType == XDriverType.KAU_Shleif || device.DriverType == XDriverType.RSR2_KAU_Shleif)
				{
					OnObjectStateChanged(device);
				}
			}
			foreach (var zone in XManager.Zones)
			{
				if (zone.GkDatabaseParent == gkDevice)
				{
					OnObjectStateChanged(zone);
				}
			}
			foreach (var direction in XManager.Directions)
			{
				if (direction.GkDatabaseParent == gkDevice)
				{
					OnObjectStateChanged(direction);
				}
			}
			foreach (var pumpStation in XManager.PumpStations)
			{
				if (pumpStation.GkDatabaseParent == gkDevice)
				{
					OnObjectStateChanged(pumpStation);
				}
			}
			foreach (var delay in XManager.Delays)
			{
				if (delay.GkDatabaseParent == gkDevice)
				{
					OnObjectStateChanged(delay);
				}
			}
			foreach (var pim in XManager.Pims)
			{
				if (pim.GkDatabaseParent == gkDevice)
				{
					OnObjectStateChanged(pim);
				}
			}
		}

		void CheckDelays()
		{
			foreach (var device in XManager.Devices)
			{
				if (!device.Driver.IsGroupDevice && device.AllParents.Any(x => x.DriverType == XDriverType.RSR2_KAU))
				{
					CheckDelay(device);
				}
			}
			foreach (var direction in XManager.Directions)
			{
				CheckDelay(direction);
			}
			foreach (var pumpStation in XManager.PumpStations)
			{
				CheckDelay(pumpStation);
			}
			foreach (var delay in XManager.Delays)
			{
				CheckDelay(delay);
			}
		}

		void CheckDelay(XBase xBase)
		{
			bool mustGetState = false;
			switch (xBase.BaseState.StateClass)
			{
				case XStateClass.TurningOn:
					mustGetState = (DateTime.Now - xBase.BaseState.LastDateTime).TotalMilliseconds > 500;
					break;
				case XStateClass.On:
					mustGetState = xBase.BaseState.ZeroHoldDelayCount < 10 && (DateTime.Now - xBase.BaseState.LastDateTime).TotalMilliseconds > 500;
					break;
				case XStateClass.TurningOff:
					mustGetState = (DateTime.Now - xBase.BaseState.LastDateTime).TotalMilliseconds > 500;
					break;
			}
			if (mustGetState)
			{
				var onDelay = xBase.BaseState.OnDelay;
				var holdDelay = xBase.BaseState.HoldDelay;
				var offDelay = xBase.BaseState.OffDelay;

				if (MeasureDeviceInfos.Any(x => x.Device.UID == xBase.BaseUID))
				{
					GetDelays(xBase);
				}
				else
				{
					GetState(xBase, true);
				}

				if (onDelay != xBase.BaseState.OnDelay || holdDelay != xBase.BaseState.HoldDelay || offDelay != xBase.BaseState.OffDelay)
					OnObjectStateChanged(xBase);

				if (xBase.BaseState.StateClass == XStateClass.On && holdDelay == 0)
					xBase.BaseState.ZeroHoldDelayCount++;
				else
					xBase.BaseState.ZeroHoldDelayCount = 0;
			}
		}

		bool GetDelays(XBase xBase)
		{
			SendResult sendResult = null;
			var expectedBytesCount = 68;
			if (xBase.KauDatabaseParent != null)
			{
				sendResult = SendManager.Send(xBase.KauDatabaseParent, 2, 12, 32, BytesHelper.ShortToBytes(xBase.KAUDescriptorNo));
				expectedBytesCount = 32;
			}
			else
			{
				sendResult = SendManager.Send(xBase.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(xBase.GKDescriptorNo));
				expectedBytesCount = 68;
			}

			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return false;
			}
			if (sendResult.Bytes.Count != expectedBytesCount)
			{
				IsDBMissmatchDuringMonitoring = true;
				xBase.BaseState.IsGKMissmatch = true;
				DBMissmatchDuringMonitoringReason = "Не совпадает количество байт в пришедшем ответе";
				return false;
			}
			ConnectionChanged(true);
			var descriptorStateHelper = new DescriptorStateHelper();
			descriptorStateHelper.Parse(sendResult.Bytes, xBase);

			xBase.BaseState.LastDateTime = DateTime.Now;
			xBase.BaseState.OnDelay = descriptorStateHelper.OnDelay;
			xBase.BaseState.HoldDelay = descriptorStateHelper.HoldDelay;
			xBase.BaseState.OffDelay = descriptorStateHelper.OffDelay;
			return true;
		}

		bool GetState(XBase xBase, bool delaysOnly = false)
		{
			var sendResult = SendManager.Send(xBase.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(xBase.GKDescriptorNo));
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return false;
			}
			if (sendResult.Bytes.Count != 68)
			{
				IsDBMissmatchDuringMonitoring = true;
				xBase.BaseState.IsGKMissmatch = true;
				DBMissmatchDuringMonitoringReason = "Не совпадает количество байт в пришедшем ответе";
				return false;
			}
			ConnectionChanged(true);
			var descriptorStateHelper = new DescriptorStateHelper();
			descriptorStateHelper.Parse(sendResult.Bytes, xBase);
			CheckDBMissmatch(xBase, descriptorStateHelper);

			xBase.BaseState.LastDateTime = DateTime.Now;
			if (!delaysOnly)
			{
				xBase.BaseState.StateBits = descriptorStateHelper.StateBits;
				xBase.BaseState.AdditionalStates = descriptorStateHelper.AdditionalStates;
			}
			xBase.BaseState.OnDelay = descriptorStateHelper.OnDelay;
			xBase.BaseState.HoldDelay = descriptorStateHelper.HoldDelay;
			xBase.BaseState.OffDelay = descriptorStateHelper.OffDelay;
			return true;
		}

		void CheckDBMissmatch(XBase xBase, DescriptorStateHelper descriptorStateHelper)
		{
			bool isMissmatch = false;
			if (xBase is XDevice)
			{
				var device = xBase as XDevice;
				if (device.Driver.DriverTypeNo != descriptorStateHelper.TypeNo)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = "Не совпадает тип устройства";
				}

				ushort physicalAddress = device.IntAddress;
				if (device.Driver.IsDeviceOnShleif)
					physicalAddress = (ushort)((device.ShleifNo - 1) * 256 + device.IntAddress);
				if (device.DriverType != XDriverType.GK && device.DriverType != XDriverType.KAU && device.DriverType != XDriverType.RSR2_KAU
					&& device.Driver.HasAddress && physicalAddress != descriptorStateHelper.PhysicalAddress)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = "Не совпадает физический адрес устройства";
				}

				var nearestDescriptorNo = 0;
				if (device.KauDatabaseParent != null)
					nearestDescriptorNo = device.KAUDescriptorNo;
				else if (device.GkDatabaseParent != null)
					nearestDescriptorNo = device.GKDescriptorNo;
				if (nearestDescriptorNo != descriptorStateHelper.AddressOnController)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = "Не совпадает адрес на контроллере";
				}
			}
			if (xBase is XZone)
			{
				if (descriptorStateHelper.TypeNo != 0x100)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = "Не совпадает тип для зоны";
				}
			}
			if (xBase is XDirection)
			{
				if (descriptorStateHelper.TypeNo != 0x106)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = "Не совпадает тип для направления";
				}
			}
			if (xBase is XPumpStation)
			{
				if (descriptorStateHelper.TypeNo != 0x106)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = "Не совпадает тип для НС";
				}
			}
			if (xBase is XDelay)
			{
				if (descriptorStateHelper.TypeNo != 0x101)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = "Не совпадает тип для Задержки";
				}
			}
			if (xBase is XPim)
			{
				if (descriptorStateHelper.TypeNo != 0x107)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = "Не совпадает тип для ПИМ";
				}
			}

			var stringLength = Math.Min(xBase.PresentationName.Length, 32);
			var description = xBase.PresentationName.Substring(0, stringLength);
			if (description.TrimEnd(' ') != descriptorStateHelper.Description)
			{
				isMissmatch = true;
				DBMissmatchDuringMonitoringReason = "Не совпадает описание компонента";
			}

			xBase.BaseState.IsRealMissmatch = isMissmatch;
			if (isMissmatch)
			{
				IsDBMissmatchDuringMonitoring = true;
			}
		}

		void CheckServiceRequired(XBase xBase, JournalItem journalItem)
		{
			if (journalItem.Name == "Запыленность" || journalItem.Name == "Запыленность устранена")
			{
				if (xBase is XDevice)
				{
					var device = xBase as XDevice;
					if (journalItem.Name == "Запыленность")
						device.InternalState.IsService = true;
					if (journalItem.Name == "Запыленность устранена")
						device.InternalState.IsService = false;
				}
			}
		}

		#region TechnologicalRegime
		void CheckTechnologicalRegime()
		{
			var isInTechnologicalRegime = DeviceBytesHelper.IsInTechnologicalRegime(GkDatabase.RootDevice);
			foreach (var descriptor in GkDatabase.Descriptors)
			{
				descriptor.XBase.BaseState.IsInTechnologicalRegime = isInTechnologicalRegime;
			}

			if (!isInTechnologicalRegime)
			{
				foreach (var kauDatabase in GkDatabase.KauDatabases)
				{
					isInTechnologicalRegime = DeviceBytesHelper.IsInTechnologicalRegime(kauDatabase.RootDevice);
					var allChildren = XManager.GetAllDeviceChildren(kauDatabase.RootDevice);
					foreach (var device in allChildren)
					{
						device.BaseState.IsInTechnologicalRegime = isInTechnologicalRegime;
					}
				}
			}
		}
		#endregion
	}
}