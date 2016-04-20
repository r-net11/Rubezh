using RubezhAPI.GK;
using System.Linq;

namespace GKProcessor
{
	public class MPTCreator
	{
		GKMPT MPT;

		public MPTCreator(GKMPT mpt)
		{
			MPT = mpt;

			foreach (var mptDevice in MPT.MPTDevices.Where(x => x.Device != null))
			{
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.HandStart ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.HandStop ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.HandAutomaticOn ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.HandAutomaticOff)
				{
					MPT.LinkToDescriptor(mptDevice.Device);
				}

				if (mptDevice.MPTDeviceType == GKMPTDeviceType.AutomaticOffBoard ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.DoNotEnterBoard ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.ExitBoard ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.Speaker ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.Bomb)
				{
					mptDevice.Device.LinkToDescriptor(MPT);
				}
			}
		}
	}
}