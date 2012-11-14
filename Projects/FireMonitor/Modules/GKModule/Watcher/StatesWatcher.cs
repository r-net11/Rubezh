using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Common;
using Common.GK;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using XFiresecAPI;
using System.Windows.Threading;
using GKModule.ViewModels;
using System.Diagnostics;

namespace GKModule
{
	public static class StatesWatcher
	{
		public static void RequestAllStates()
		{
            var thread = new Thread(new ThreadStart(OnRun));
            thread.Start();
			Dispatcher.CurrentDispatcher.ShutdownStarted += (s, e) =>
				{
					if (thread.IsAlive)
						thread.Abort();
				};
		}

        static void OnRun()
        {
            foreach (var gkDatabase in DatabaseManager.GkDatabases)
            {
                if (gkDatabase.RootDevice.GetGKIpAddress() == null)
                    continue;

                foreach (var binaryObject in gkDatabase.BinaryObjects)
                {
                    GetState(binaryObject.BinaryBase, gkDatabase.RootDevice);
                }
				//foreach (var binaryObject in gkDatabase.BinaryObjects)
				//{
				//    GetParameters(binaryObject.BinaryBase, gkDatabase.RootDevice);
				//}
            }
			ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Publish(null); });
        }

		public static void RequestObjectState(XBinaryBase binaryBase)
		{
			GetState(binaryBase, binaryBase.GkDatabaseParent);
		}

		static void GetState(XBinaryBase binaryBase, XDevice gkParent)
		{
			var no = binaryBase.GetDatabaseNo(DatabaseType.Gk);
			var sendResult = SendManager.Send(gkParent, 2, 12, 68, BytesHelper.ShortToBytes(no));
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return;
			}
			if (sendResult.Bytes.Count == 68)
			{
				var binaryObjectState = new BinaryObjectState(sendResult.Bytes);
				ApplicationService.Invoke(() => { SetObjectStates(binaryBase, binaryObjectState.States); });
			}
			else
			{
				Logger.Error("StatesWatcher.GetState sendResult.Bytes.Count != 68");
			}
		}

		static void GetParameters(XBinaryBase binaryBase, XDevice gkParent)
		{
			var device = binaryBase as XDevice;
			if (device == null)
				return;

			var AUParameterValues = new List<AUParameterValue>();
			foreach (var auParameter in device.Driver.AUParameters)
			{
				var bytes = new List<byte>();
				var databaseNo = device.GetDatabaseNo(DatabaseType.Kau);
				bytes.Add((byte)device.Driver.DriverTypeNo);
				bytes.Add(device.IntAddress);
				bytes.Add((byte)(device.ShleifNo - 1));
				bytes.Add(auParameter.No);
				var result = SendManager.Send(device.KauDatabaseParent, 4, 128, 2, bytes);
				if (!result.HasError)
				{
					if (result.Bytes.Count > 0)
					{
						var parameterValue = BytesHelper.SubstructShort(result.Bytes, 0);
						var auParameterValue = new AUParameterValue()
						{
							Name = auParameter.Name,
							Value = parameterValue
						};
						AUParameterValues.Add(auParameterValue);
					}
				}
			}

			var currentDustinessParameter = AUParameterValues.FirstOrDefault(x => x.Name == "Текущая запыленность");
			var criticalDustinessParameter = AUParameterValues.FirstOrDefault(x => x.Name == "Порог запыленности предварительный");
			if (currentDustinessParameter != null && criticalDustinessParameter != null)
			{
				if (currentDustinessParameter.Value > 0 && currentDustinessParameter.Value > 0)
				{
					if (currentDustinessParameter.Value - criticalDustinessParameter.Value > 0)
					{
						ApplicationService.Invoke(() => { device.DeviceState.IsService = true; });
					}
				}
			}
		}

		public static void SetObjectStates(XBinaryBase binaryBase, List<XStateType> states)
		{
			if (binaryBase is XDevice)
			{
				var device = binaryBase as XDevice;
				device.DeviceState.States = states;
			}
			if (binaryBase is XZone)
			{
				var zone = binaryBase as XZone;
				zone.ZoneState.States = states;
			}
			if (binaryBase is XDirection)
			{
				var direction = binaryBase as XDirection;
				direction.DirectionState.States = states;
			}
		}

		public static void CheckServiceRequired(XBinaryBase binaryBase, JournalItem journalItem)
		{
			if(journalItem.Name != "Запыленность")
				return;

			if (binaryBase is XDevice)
			{
				var device = binaryBase as XDevice;
				bool isDusted = journalItem.YesNo == "Есть";
				ApplicationService.Invoke(() => { device.DeviceState.IsService = isDusted; });
			}
		}

		static void ConnectionChanged(bool value)
		{
			ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<GKConnectionChanged>().Publish(value); });
		}
	}
}