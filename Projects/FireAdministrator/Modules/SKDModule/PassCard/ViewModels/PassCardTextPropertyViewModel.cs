using Infrastructure.Designer.ElementProperties.ViewModels;
using FiresecAPI.SKD.PassCardLibrary;
using System;

namespace SKDModule.PassCard.ViewModels
{
	public class PassCardTextPropertyViewModel : TextBlockPropertiesViewModel
	{
		public PassCardTextPropertyViewModel(ElementPassCardTextProperty element)
			: base(element)
		{
			element.Property = Guid.NewGuid().ToString();
		}
	}
}
