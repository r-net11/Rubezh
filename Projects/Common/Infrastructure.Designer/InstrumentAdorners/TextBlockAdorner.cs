using Infrastructure.Common.Windows;
using Infrastructure.Designer.ElementProperties.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class TextBlockAdorner : RectangleAdorner
	{
		public TextBlockAdorner(BaseDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementTextBlock();
			var propertiesViewModel = new TextBlockPropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}