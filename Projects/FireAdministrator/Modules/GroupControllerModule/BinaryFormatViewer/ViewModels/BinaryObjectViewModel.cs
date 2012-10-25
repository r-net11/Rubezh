using System.Linq;
using Common.GK;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class BinaryObjectViewModel : BaseViewModel
	{
		public BinaryObjectViewModel(BinaryObjectBase binaryObject)
		{
			BinaryObject = binaryObject;
			Description = binaryObject.BinaryBase.GetBinaryDescription();
			switch (binaryObject.ObjectType)
			{
				case ObjectType.Device:
					ImageSource = binaryObject.Device.Driver.ImageSource;
					break;

				case ObjectType.Zone:
				case ObjectType.Direction:
					ImageSource = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == XDriverType.System).ImageSource;
					break;
			}

			Formula = BinaryObject.Formula.GetStringFomula();
		}

		public BinaryObjectBase BinaryObject { get; set; }
		public string ImageSource { get; set; }
		public string Description { get; set; }
		public string Formula { get; set; }
	}
}