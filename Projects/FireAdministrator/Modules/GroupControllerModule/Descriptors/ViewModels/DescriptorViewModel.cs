using GKProcessor;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DescriptorViewModel : BaseViewModel
	{
		public DescriptorViewModel(BaseDescriptor descriptor)
		{
			Descriptor = descriptor;
			Description = descriptor.XBase.PresentationName;
			switch (descriptor.DescriptorType)
			{
				case DescriptorType.Device:
					ImageSource = descriptor.Device.Driver.ImageSource;
					break;

				case DescriptorType.Zone:
					ImageSource = "/Controls;component/Images/Zone.png";
					break;

				case DescriptorType.Direction:
					ImageSource = "/Controls;component/Images/Blue_Direction.png";
					break;

				case DescriptorType.PumpStation:
					ImageSource = "/Controls;component/Images/Blue_Direction.png";
					break;

				case DescriptorType.Delay:
					ImageSource = "/Controls;component/Images/Delay.png";
					break;

				case DescriptorType.Pim:
					ImageSource = null;
					break;
			}

			IsFormulaInvalid = Descriptor.Formula.CalculateStackLevels();
		}

		public BaseDescriptor Descriptor { get; set; }
		public string ImageSource { get; set; }
		public string Description { get; set; }
		public bool IsFormulaInvalid { get; set; }
	}
}