﻿using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI.GK;
using FiresecAPI.Models;
using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.InstrumentAdorners;

namespace GKModule.Plans.InstrumentAdorners
{
	public class DirectionPolygonAdorner : BasePolygonAdorner
	{
		private DirectionsViewModel _directionsViewModel;
		public DirectionPolygonAdorner(CommonDesignerCanvas designerCanvas, DirectionsViewModel directionsViewModel)
			: base(designerCanvas)
		{
			_directionsViewModel = directionsViewModel;
		}

		protected override Shape CreateRubberband()
		{
			return new Polygon();
		}
		protected override PointCollection Points
		{
			get { return ((Polygon)Rubberband).Points; }
		}
		protected override ElementBaseShape CreateElement()
		{
			var element = new ElementPolygonGKDirection();
			var propertiesViewModel = new DirectionPropertiesViewModel(element, _directionsViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			GKPlanExtension.Instance.SetItem<GKDirection>(element);
			return element;
		}
	}
}