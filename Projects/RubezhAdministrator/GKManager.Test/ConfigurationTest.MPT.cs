using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhAPI;
using RubezhAPI.GK;
using System;
using System.Linq;

namespace GKManager2.Test
{
	public partial class ConfigurationTest
	{
		/// <summary>
		/// Устройства МПТ должны иметь настроенные устройства
		/// </summary>
		[TestMethod]
		public void ValidateEmptyMPTDeviceDevice()
		{
			var mpt = new GKMPT();
			var emptyMptDevice = (new GKMPTDevice { MPTDeviceType = GKMPTDeviceType.Bomb });
            mpt.MPTDevices.Add(emptyMptDevice);
			GKManager.AddMPT(mpt);
			mpt.Invalidate(GKManager.DeviceConfiguration);
			Assert.IsTrue(mpt.MPTDevices.FirstOrDefault(x => x == emptyMptDevice) == null);
		}
	}
}
