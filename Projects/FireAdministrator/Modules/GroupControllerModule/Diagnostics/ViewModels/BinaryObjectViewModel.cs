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
					ImageSource = "/Controls;component/Images/zone.png";
					break;

				case ObjectType.Direction:
					ImageSource = "/Controls;component/Images/Blue_Direction.png";
					break;

				case ObjectType.Delay:
					ImageSource = "/Controls;component/Images/Delay.png";
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