using GKModule.Models;
using Infrastructure.Common;
using XFiresecAPI;
using GKModule.Converter;

namespace GKModule.ViewModels
{
	public class BinObjectViewModel : BaseViewModel
	{
		public BinObjectViewModel(XDevice device)
		{
			DeviceName = device.Driver.ShortName;
			DeviceAddress = device.Address;
			ImageSource = device.Driver.ImageSource;
			Level = device.AllParents.Count;
			DeviceBinaryFormatter = new KauBinaryObject();
			DeviceBinaryFormatter.Initialize(device);
		}

		public string DeviceName { get; set; }
		public string DeviceAddress { get; set; }
		public string ImageSource { get; set; }
		public int Level { get; set; }
		public KauBinaryObject DeviceBinaryFormatter { get; set; }
	}
}