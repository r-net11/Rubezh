using System;
using RubezhAPI.GK;
namespace GKProcessor
{
	public  class GKMirrorPerformersDevice
	
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x998,
				DriverType = GKDriverType.RSR2_GKMirrorPerformersDevice,
				UID = new Guid("E4850E38-1800-4024-B764-B8CF6D9AAD49"),
				Name = "Исполнительные устройства",
				ShortName = "Исполнительные устройства",
				HasAddress = true,
				IsDeviceOnShleif = false,
				IsPlaceable = false,
				HasMirror = true,
				MinAddress = 1,
				MaxAddress = 2000
			};

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.On);

			return driver;
		}
	}

	}

