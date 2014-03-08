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
			Title = "Свойства фигуры: Текстовое свойство";
			element.PropertyType = PassCardPropertyType.Additional;
		}
	}
}
