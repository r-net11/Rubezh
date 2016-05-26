﻿using StrazhAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Designer.ElementProperties.ViewModels;
using StrazhAPI.Plans.Elements;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class TextBoxAdorner : RectangleAdorner
	{
		public TextBoxAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override StrazhAPI.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementTextBlock();
			var propertiesViewModel = new TextBlockPropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}