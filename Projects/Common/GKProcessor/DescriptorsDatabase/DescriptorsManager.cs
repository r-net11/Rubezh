﻿using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;

namespace GKProcessor
{
	public static class DescriptorsManager
	{
		public static List<KauDatabase> KauDatabases { get; private set; }
		public static List<GkDatabase> GkDatabases { get; private set; }

		public static void Create()
		{
			GKManager.DeviceConfiguration.PrepareDescriptors();
			GKManager.DeviceConfiguration.UpdateConfiguration();

			GkDatabases = new List<GkDatabase>();
			KauDatabases = new List<KauDatabase>();

			foreach (var device in GKManager.Devices.Where(x => x.DriverType == GKDriverType.GK))
			{
				var gkDatabase = new GkDatabase(device);
				GkDatabases.Add(gkDatabase);

				foreach (var kauDevice in device.Children.Where(x => x.Driver.IsKau))
				{
					var kauDatabase = new KauDatabase(kauDevice);
					gkDatabase.KauDatabases.Add(kauDatabase);
					KauDatabases.Add(kauDatabase);
				}
			}

			KauDatabases.ForEach(x => x.BuildObjects());
			GkDatabases.ForEach(x => x.BuildObjects());
			CreateDynamicObjectsInGKManager();

			//using (var textWriter = new System.IO.StreamWriter(@"D:\Bytes.txt"))
			//{
			//	foreach (var database in GkDatabases)
			//	{
			//		foreach (var descriptor in database.Descriptors)
			//		{
			//			textWriter.WriteLine(BytesHelper.BytesToString(descriptor.AllBytes));
			//		}
			//	}
			//	foreach (var database in KauDatabases)
			//	{
			//		foreach (var descriptor in database.Descriptors)
			//		{
			//			textWriter.WriteLine(BytesHelper.BytesToString(descriptor.AllBytes));
			//		}
			//	}
			//}
		}

		public static void CreateDynamicObjectsInGKManager()
		{
			GKManager.AutoGeneratedDelays = new List<GKDelay>();
			GKManager.AutoGeneratedPims = new List<GKPim>();

			foreach (var gkDatabase in GkDatabases)
			{
				foreach (var descriptor in gkDatabase.Descriptors)
				{
					if (descriptor is DelayDescriptor)
					{
						var delay = descriptor.GKBase as GKDelay;
						if (delay != null)
						{
							descriptor.GKBase.InternalState = new GKDelayInternalState(delay);
							descriptor.GKBase.State = new GKState(delay);

							if (delay.IsAutoGenerated)
							{
								GKManager.AutoGeneratedDelays.Add(delay);
							}
						}
					}
					if (descriptor is PimDescriptor)
					{
						var pim = descriptor.GKBase as GKPim;
						if (pim != null)
						{
							descriptor.GKBase.InternalState = new GKPimInternalState(pim);
							descriptor.GKBase.State = new GKState(pim);

							if (pim != null && pim.IsAutoGenerated)
							{
								GKManager.AutoGeneratedPims.Add(pim);
							}
						}
					}
				}
			}
		}

		public static IEnumerable<DescriptorError> Check()
		{
			foreach (var kauDatabase in KauDatabases)
			{
				foreach (var descriptorError in kauDatabase.Check())
					yield return descriptorError;
			}
			foreach (var gkDatabase in GkDatabases)
			{
				foreach (var descriptorError in gkDatabase.Check())
					yield return descriptorError;
			}
		}
	}
}