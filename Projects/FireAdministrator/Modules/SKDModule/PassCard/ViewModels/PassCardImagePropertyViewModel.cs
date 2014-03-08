using System;
using FiresecAPI.SKD.PassCardLibrary;
using Infrastructure.Designer.ElementProperties.ViewModels;

namespace SKDModule.PassCard.ViewModels
{
	public class PassCardImagePropertyViewModel : RectanglePropertiesViewModel
	{
		public PassCardImagePropertyViewModel(ElementPassCardImageProperty element)
			: base(element)
		{
			Title = "Свойства фигуры: Графическое свойство";
			element.PropertyType = PassCardPropertyType.Additional;
		}
	}
}
