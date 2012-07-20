using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using PlansModule.Designer;
using Infrustructure.Plans.Elements;
using FiresecAPI.Models;
using PlansModule.ViewModels;
using Infrastructure.Common.Windows;

namespace PlansModule.InstrumentAdorners
{
	public class ZonePolygonAdorner : PolygonAdorner
	{
		public ZonePolygonAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override PointCollection Points
		{
			get { return ((Polygon)Rubberband).Points; }
		}
		protected override ElementBaseShape CreateElement()
		{
			var element = new ElementPolygonZone();
			var propertiesViewModel = new ZonePropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}