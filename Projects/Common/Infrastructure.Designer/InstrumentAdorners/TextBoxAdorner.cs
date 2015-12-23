﻿using RubezhAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Designer.ElementProperties.ViewModels;
using Infrustructure.Plans.Elements;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class TextBoxAdorner : RectangleAdorner
	{
		public TextBoxAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementTextBox();
			var propertiesViewModel = new TextBoxPropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}