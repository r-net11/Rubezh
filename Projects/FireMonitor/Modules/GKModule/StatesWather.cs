using System.Linq;
using Commom.GK;
using Common.GK;
using FiresecAPI.Models;
using FiresecClient;

namespace GKModule
{
	public static class StatesWather
	{
		public static void Run()
		{
			DatabaseProcessor.Convert();
			foreach (var gkDatabase in DatabaseProcessor.DatabaseCollection.GkDatabases)
			{
				GetStatesFromDB(gkDatabase);
			}

			foreach (var kauDatabase in DatabaseProcessor.DatabaseCollection.KauDatabases)
			{
				GetStatesFromDB(kauDatabase);
			}
		}

		static void GetStatesFromDB(CommonDatabase commonDatabase)
		{
			foreach (var binaryObject in commonDatabase.BinaryObjects)
			{
				var rootDevice = commonDatabase.RootDevice;
				var no = binaryObject.GetNo();
				var bytes = SendManager.Send(rootDevice, 2, 12, 68, BytesHelper.ShortToBytes(no));
				if (bytes.Count > 0)
				{
					if (binaryObject.Device != null)
					{
						var deviceState = XManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == binaryObject.Device.UID);
						if (deviceState != null)
						{
							var binaryDeviceState = new BinaryDeviceState(bytes, binaryObject.DatabaseType);
							deviceState.States = binaryDeviceState.States;
							var minPriority = 7;
							foreach (var state in deviceState.States)
							{
								var priority = StatesHelper.XStateTypeToPriority(state);
								if (priority < minPriority)
								{
									minPriority = priority;
								}
							}
							deviceState.StateType = (StateType)minPriority;
							deviceState.OnStateChanged();
						}
					}
				}
			}
		}
	}
}