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

namespace GKProcessor
{
	public partial class Watcher
	{
		bool IsAnyDBMissmatch = false;

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
						SystemDateTime = DateTime.Now,
						DeviceDateTime = DateTime.Now,
						GKIpAddress = XManager.GetIpAddress(GkDatabase.RootDevice),
						JournalItemType = JournalItemType.System,
						StateClass = XStateClass.Unknown,
						ObjectStateClass = XStateClass.Norm,
						Name = value ? "База данных прибора не соответствует базе данных ПК" : "База данных прибора соответствует базе данных ПК"
					};
					AddJournalItem(journalItem);
				}
			}
		}

		void GetAllStates(bool showProgress)
		{
			IsAnyDBMissmatch = false;

			if (showProgress)
				GKProcessorManager.OnStartProgress("Опрос объектов ГК", "", GkDatabase.Descriptors.Count, false);
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
			}
			foreach (var device in XManager.Devices)
			{
				if (device.DriverType == XDriverType.KAU_Shleif || device.DriverType == XDriverType.RSR2_KAU_Shleif)
				{
					device.InternalState.OnInternalStateChanged();
				}
			}
			if (showProgress)
				GKProcessorManager.OnStopProgress();

			if (IsAnyDBMissmatch)
			{
				foreach (var descriptor in GkDatabase.Descriptors)
				{
					descriptor.XBase.BaseState.StateBits = new List<XStateBit>() { XStateBit.Norm };
					descriptor.XBase.BaseState.IsGKMissmatch = true;
				}
			}
			else
			{
				foreach (var descriptor in GkDatabase.Descriptors)
				{
					descriptor.XBase.BaseState.IsGKMissmatch = false;
				}
			}
			IsJournalAnyDBMissmatch = IsAnyDBMissmatch;
			CheckTechnologicalRegime();

			var gkDevice = XManager.Devices.FirstOrDefault(x => x.UID == GkDatabase.RootDevice.UID);
			foreach (var device in XManager.GetAllDeviceChildren(gkDevice))
			{
				OnObjectStateChanged(device);
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
                    mustGetState = xBase.BaseState.OnDelay > 0 || (DateTime.Now - xBase.BaseState.LastDateTime).Seconds > 1;
                    break;
                case XStateClass.On:
                    mustGetState = xBase.BaseState.HoldDelay > 0 || (DateTime.Now - xBase.BaseState.LastDateTime).Seconds > 1;
                    break;
                case XStateClass.TurningOff:
                    mustGetState = xBase.BaseState.OffDelay > 0 || (DateTime.Now - xBase.BaseState.LastDateTime).Seconds > 1;
                    break;
            }
            if (mustGetState)
            {
                var onDelay = xBase.BaseState.OnDelay;
                var holdDelay = xBase.BaseState.HoldDelay;
                var offDelay = xBase.BaseState.OffDelay;
                GetState(xBase, true);
                if (onDelay != xBase.BaseState.OnDelay || holdDelay != xBase.BaseState.HoldDelay || offDelay != xBase.BaseState.OffDelay)
                    OnObjectStateChanged(xBase);
            }
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
				IsAnyDBMissmatch = true;
				xBase.BaseState.IsGKMissmatch = true;
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
					isMissmatch = true;

				ushort physicalAddress = device.IntAddress;
				if (device.Driver.IsDeviceOnShleif)
					physicalAddress = (ushort)((device.ShleifNo - 1) * 256 + device.IntAddress);
				if (device.DriverType != XDriverType.GK && device.DriverType != XDriverType.KAU && device.DriverType != XDriverType.RSR2_KAU
					&& device.Driver.HasAddress && physicalAddress != descriptorStateHelper.PhysicalAddress)
					isMissmatch = true;

				var nearestDescriptorNo = 0;
				if (device.KauDatabaseParent != null)
					nearestDescriptorNo = device.KAUDescriptorNo;
				else if (device.GkDatabaseParent != null)
					nearestDescriptorNo = device.GKDescriptorNo;
				if (nearestDescriptorNo != descriptorStateHelper.AddressOnController)
					isMissmatch = true;
			}
			if (xBase is XZone)
			{
				var zone = xBase as XZone;
				if (descriptorStateHelper.TypeNo != 0x100)
					isMissmatch = true;
			}
			if (xBase is XDirection)
			{
				var direction = xBase as XDirection;
				if (descriptorStateHelper.TypeNo != 0x106)
					isMissmatch = true;
			}
			if (xBase is XPumpStation)
			{
				var pumpStation = xBase as XPumpStation;
				if (descriptorStateHelper.TypeNo != 0x106)
					isMissmatch = true;
			}
			if (xBase is XDelay)
			{
				var delay = xBase as XDelay;
				if (descriptorStateHelper.TypeNo != 0x101)
					isMissmatch = true;
			}

			var description = xBase.PresentationName;
			if (xBase.PresentationName.TrimEnd(' ') != descriptorStateHelper.Description)
				isMissmatch = true;

			xBase.BaseState.IsRealMissmatch = isMissmatch;
			if (isMissmatch)
			{
				IsAnyDBMissmatch = true;
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