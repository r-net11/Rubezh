﻿using System.Windows.Media;
using System.Windows.Shapes;
using RubezhAPI.GK;
using RubezhAPI.Models;
using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.InstrumentAdorners;

namespace GKModule.Plans.InstrumentAdorners
{
	public class GuardZonePolygonAdorner : BasePolygonAdorner
	{
		GuardZonesViewModel _guardZonesViewModel;

		public GuardZonePolygonAdorner(CommonDesignerCanvas designerCanvas, GuardZonesViewModel guardZonesViewModel)
			: base(designerCanvas)
		{
			_guardZonesViewModel = guardZonesViewModel;
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
			var element = new ElementPolygonGKGuardZone();
			var propertiesViewModel = new GuardZonePropertiesViewModel(element, _guardZonesViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			GKPlanExtension.Instance.SetItem<GKGuardZone>(element);
			return element;
		}
	}
}