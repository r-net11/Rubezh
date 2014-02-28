using Infrastructure.Designer.ElementProperties.ViewModels;
using FiresecAPI.SKD.PassCardLibrary;
using System;

namespace SKDModule.PassCard.ViewModels
{
	public class PassCardImagePropertyViewModel : RectanglePropertiesViewModel
	{
		public PassCardImagePropertyViewModel(ElementPassCardImageProperty element)
			: base(element)
		{
			element.Property = Guid.NewGuid().ToString();
		}
	}
}
