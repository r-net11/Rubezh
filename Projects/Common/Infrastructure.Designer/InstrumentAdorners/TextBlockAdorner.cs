using Infrastructure.Common.Windows;
using Infrastructure.Designer.ElementProperties.ViewModels;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class TextBlockAdorner : RectangleAdorner
	{
		public TextBlockAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementTextBlock();
			var propertiesViewModel = new TextBlockPropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}