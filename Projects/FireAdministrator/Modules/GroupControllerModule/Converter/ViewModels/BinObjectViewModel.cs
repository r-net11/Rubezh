using GKModule.Models;
using Infrastructure.Common;
using XFiresecAPI;
using GKModule.Converter;
using GKModule.Converter.Binary;

namespace GKModule.ViewModels
{
	public class BinObjectViewModel : BaseViewModel
	{
		public BinObjectViewModel(XDevice device)
		{
			Name = device.Driver.ShortName;
			Address = device.Address;
			ImageSource = device.Driver.ImageSource;
			Level = device.AllParents.Count;
			BinaryObject = new KauBinaryObject(device);
		}

		public BinObjectViewModel(XZone zone)
		{
			Name = zone.Name;
			Address = zone.No.ToString();
			ImageSource = null;
			Level = 3;
			BinaryObject = new ZoneBinaryObject(zone);
		}

		public string Name { get; set; }
		public string Address { get; set; }
		public string ImageSource { get; set; }
		public int Level { get; set; }
		public BinaryObjectBase BinaryObject { get; set; }
	}
}