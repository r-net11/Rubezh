﻿using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;
using GKProcessor.DescriptorsDatabase;

namespace GKProcessor
{
	public static class DescriptorsManager
	{
		public static List<KauDatabase> KauDatabases { get; private set; }
		public static List<GkDatabase> GkDatabases { get; private set; }

		public static void Create()
		{
			GKManager.UpdateConfiguration();
			GKManager.PrepareDescriptors();

			var testDescriptorsCreator = new TestDescriptorsCreator();
			testDescriptorsCreator.SetDependentDescriptors();

			GkDatabases = new List<GkDatabase>();
			KauDatabases = new List<KauDatabase>();

			foreach (var device in GKManager.Devices)
			{
				if (device.DriverType == GKDriverType.GK)
				{
					var gkDatabase = new GkDatabase(device);
					GkDatabases.Add(gkDatabase);

					foreach (var kauDevice in device.Children)
					{
						if (kauDevice.Driver.IsKau)
						{
							var kauDatabase = new KauDatabase(kauDevice);
							gkDatabase.KauDatabases.Add(kauDatabase);
							KauDatabases.Add(kauDatabase);
						}
					}
				}
			}

			KauDatabases.ForEach(x => x.BuildObjects());
			GkDatabases.ForEach(x => x.BuildObjects());
			CreateDynamicObjectsInGKManager();

			Check();
		}

		public static void CreateDynamicObjectsInGKManager()
		{
			GKManager.AutoGeneratedDelays = new List<GKDelay>();
			GKManager.AutoGeneratedPims = new List<GKPim>();
			foreach (var gkDatabase in GkDatabases)
			{
				foreach (var descriptor in gkDatabase.Descriptors)
				{
					if(descriptor is DelayDescriptor)
					{
						descriptor.GKBase.InternalState = new GKDelayInternalState(descriptor.GKBase as GKDelay);
						descriptor.GKBase.State = new GKState(descriptor.GKBase as GKDelay);
					}
					if (descriptor is PimDescriptor)
					{
						descriptor.GKBase.InternalState = new GKPimInternalState(descriptor.GKBase as GKPim);
						descriptor.GKBase.State = new GKState(descriptor.GKBase as GKPim);
					}
				}
				//foreach (var delay in gkDatabase.Delays)
				//{
				//	delay.InternalState = new GKDelayInternalState(delay);
				//	delay.State = new GKState(delay);
				//	GKManager.AutoGeneratedDelays.Add(delay);
				//}
				//foreach (var pim in gkDatabase.Pims)
				//{
				//	pim.InternalState = new GKPimInternalState(pim);
				//	pim.State = new GKState(pim);
				//	GKManager.AutoGeneratedPims.Add(pim);
				//}
			}
		}

		static void Check()
		{
			foreach(var kauDatabase in KauDatabases)
			{
				var result = kauDatabase.Check();
				if(result != null)
				{
					;
				}
			}
			foreach (var gkDatabase in GkDatabases)
			{
				var result = gkDatabase.Check();
				if (result != null)
				{
					;
				}
			}
		}
	}
}