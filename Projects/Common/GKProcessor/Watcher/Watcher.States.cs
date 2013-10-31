using System.Collections.Generic;
using Common;
using Common.GK;
using Infrastructure;
using Infrastructure.Common.Windows;
using GKProcessor.Events;
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

		void GetAllStates(bool showProgress)
		{
			Trace.WriteLine("GetAllStates");
			IsAnyDBMissmatch = false;

			if (showProgress)
				StartProgress("Опрос объектов ГК", GkDatabase.BinaryObjects.Count);
			foreach (var binaryObject in GkDatabase.BinaryObjects)
			{
				LastUpdateTime = DateTime.Now;
				var result = GetState(binaryObject.BinaryBase);
				if (!result)
				{
					if (binaryObject.Device != null && binaryObject.Device.Driver.DriverType == XDriverType.GK)
					{
						binaryObject.Device.DeviceState.IsConnectionLost = true;
						break;
					}
				}
				if (showProgress)
					DoProgress(binaryObject.BinaryBase.BinaryInfo.ToString());
			}
			foreach (var device in XManager.Devices)
			{
				if (device.Driver.DriverType == XDriverType.KAU_Shleif || device.Driver.DriverType == XDriverType.RSR2_KAU_Shleif)
				{
					device.DeviceState.OnStateChanged();
				}
			}
			if (showProgress)
				StopProgress();

			if (IsAnyDBMissmatch)
			{
				foreach (var binaryObject in GkDatabase.BinaryObjects)
				{
					var baseState = binaryObject.BinaryBase.GetXBaseState();
					baseState.StateBits = new List<XStateBit>() { XStateBit.Norm };
					baseState.IsGKMissmatch = true;
				}
			}
			else
			{
				foreach (var binaryObject in GkDatabase.BinaryObjects)
				{
					var baseState = binaryObject.BinaryBase.GetXBaseState();
					baseState.IsGKMissmatch = false;
				}
			}
			CheckTechnologicalRegime();

			ApplicationService.Invoke(() => { ServiceFactoryBase.Events.GetEvent<GKObjectsStateChangedEvent>().Publish(null); });
		}

		bool GetState(XBinaryBase binaryBase)
		{
			var no = binaryBase.GetDatabaseNo(DatabaseType.Gk);
			var sendResult = SendManager.Send(binaryBase.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(no));
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return false;
			}
			if (sendResult.Bytes.Count != 68)
			{
				IsAnyDBMissmatch = true;
				ApplicationService.Invoke(() => { binaryBase.GetXBaseState().IsGKMissmatch = true; });
				return false;
			}
			ConnectionChanged(true);
			var binaryObjectStateHelper = new BinaryObjectStateHelper();
            binaryObjectStateHelper.Parse(sendResult.Bytes);
			CheckDBMissmatch(binaryBase, binaryObjectStateHelper);
			ApplicationService.Invoke(() =>
			{
				var binaryState = binaryBase.GetXBaseState();
				binaryState.AdditionalStates = binaryObjectStateHelper.AdditionalStates;
				binaryState.AdditionalStateProperties = binaryObjectStateHelper.AdditionalStateProperties;
				binaryState.OnDelay = binaryObjectStateHelper.OnDelay;
				binaryState.HoldDelay = binaryObjectStateHelper.HoldDelay;
				binaryState.OffDelay = binaryObjectStateHelper.OffDelay;
				binaryState.LastDateTime = DateTime.Now;
				binaryState.StateBits = binaryObjectStateHelper.StateBits; // OnStateChanged();
			});

			return true;
		}

		void CheckAdditionalStates(BinaryObjectBase binaryObject)
		{
			if (binaryObject is DeviceBinaryObject)
			{
				var deviceBinaryObject = binaryObject as DeviceBinaryObject;
				if (deviceBinaryObject.Device.Driver.DriverType == XDriverType.GK || deviceBinaryObject.Device.Driver.DriverType == XDriverType.KAU)
				{
					GetState(binaryObject.BinaryBase);
				}
			}
		}

		void CheckDBMissmatch(XBinaryBase binaryBase, BinaryObjectStateHelper binaryObjectState)
		{
			bool isMissmatch = false;
			if (binaryBase is XDevice)
			{
				var device = binaryBase as XDevice;
				if (device.Driver.DriverTypeNo != binaryObjectState.TypeNo)
					isMissmatch = true;

				ushort physicalAddress = device.IntAddress;
				if (device.Driver.IsDeviceOnShleif)
					physicalAddress = (ushort)((device.ShleifNoNew - 1) * 256 + device.IntAddress);
				if (device.Driver.DriverType != XDriverType.GK && device.Driver.DriverType != XDriverType.KAU && device.Driver.DriverType != XDriverType.RSR2_KAU
					&& device.Driver.HasAddress && physicalAddress != binaryObjectState.PhysicalAddress)
					isMissmatch = true;

				if (device.GetNearestDatabaseNo() != binaryObjectState.AddressOnController)
					isMissmatch = true;
			}
			if (binaryBase is XZone)
			{
				var zone = binaryBase as XZone;
				if (binaryObjectState.TypeNo != 0x100)
					isMissmatch = true;
			}
			if (binaryBase is XDirection)
			{
				var direction = binaryBase as XDirection;
				if (binaryObjectState.TypeNo != 0x106)
					isMissmatch = true;
			}
			if (binaryBase is XDelay)
			{
				var delay = binaryBase as XDelay;
				if (binaryObjectState.TypeNo != 0x101)
					isMissmatch = true;
			}

            var description = binaryBase.GetBinaryDescription();
            if (binaryBase.GetBinaryDescription().TrimEnd(' ') != binaryObjectState.Description)
				isMissmatch = true;

			binaryBase.GetXBaseState().IsRealMissmatch = isMissmatch;
			if (isMissmatch)
			{
				IsAnyDBMissmatch = true;
			}
		}

		void CheckServiceRequired(XBinaryBase binaryBase, JournalItem journalItem)
		{
			if (journalItem.Name != "Запыленность")
				return;

			if (binaryBase is XDevice)
			{
				var device = binaryBase as XDevice;
				bool isDusted = journalItem.YesNo == JournalYesNoType.Yes;
				ApplicationService.Invoke(() => { device.DeviceState.IsService = isDusted; });
			}
		}

		#region TechnologicalRegime
		void CheckTechnologicalRegime()
		{
			var isInTechnologicalRegime = IsInTechnologicalRegime(GkDatabase.RootDevice);
			foreach (var binaryObject in GkDatabase.BinaryObjects)
			{
				var baseState = binaryObject.BinaryBase.GetXBaseState();
				baseState.IsInTechnologicalRegime = isInTechnologicalRegime;
			}

			if (!isInTechnologicalRegime)
			{
				foreach (var kauDatabase in GkDatabase.KauDatabases)
				{
					isInTechnologicalRegime = IsInTechnologicalRegime(kauDatabase.RootDevice);
					var allChildren = XManager.GetAllDeviceChildren(kauDatabase.RootDevice);
					allChildren.Add(kauDatabase.RootDevice);
					foreach (var device in allChildren)
					{
						var baseState = device.GetXBaseState();
						baseState.IsInTechnologicalRegime = isInTechnologicalRegime;
					}
				}
			}
		}

		bool IsInTechnologicalRegime(XDevice device)
		{
			var sendResult = SendManager.Send(device, 0, 1, 1, null, true, false, 2000);
			if (!sendResult.HasError)
			{
				if (sendResult.Bytes.Count > 0)
				{
					var version = sendResult.Bytes[0];
					if (version > 127)
					{
						return true;
					}
				}
			}
			return false;
		}
		#endregion
	}
}