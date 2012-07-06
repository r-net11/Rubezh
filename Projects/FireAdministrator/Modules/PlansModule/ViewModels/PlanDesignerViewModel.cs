using System.Collections.Generic;
using System.Windows.Shapes;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using PlansModule.Designer;
using PlansModule.Events;
using PlansModule.Views;
using Infrustructure.Plans.Elements;
using System;

namespace PlansModule.ViewModels
{
	public partial class PlanDesignerViewModel : BaseViewModel
	{
		public event EventHandler Updated;
		public DesignerCanvas DesignerCanvas { get; set; }
		public Plan Plan { get; private set; }

		public PlanDesignerViewModel()
		{
			InitializeZIndexCommands();
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(x => { UpdateDeviceInZones(); });
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(x => { UpdateDeviceInZones(); });
		}

		public void Initialize(Plan plan)
		{
			Plan = plan;
			OnPropertyChanged("Plan");
			if (Plan != null)
			{
				ChangeZoom(1);
				DesignerCanvas.Plan = plan;
				DesignerCanvas.PlanDesignerViewModel = this;
				DesignerCanvas.Update();
				DesignerCanvas.Children.Clear();
				DesignerCanvas.Width = plan.Width;
				DesignerCanvas.Height = plan.Height;
				OnUpdated();

				foreach (var elementRectangle in plan.ElementRectangles)
					DesignerCanvas.Create(elementRectangle);
				foreach (var elementEllipse in plan.ElementEllipses)
					DesignerCanvas.Create(elementEllipse);
				foreach (var elementTextBlock in plan.ElementTextBlocks)
					DesignerCanvas.Create(elementTextBlock);
				foreach (var elementPolygon in plan.ElementPolygons)
					DesignerCanvas.Create(elementPolygon);
				foreach (var elementPolyline in plan.ElementPolylines)
					DesignerCanvas.Create(elementPolyline);
				foreach (var elementRectangleZone in plan.ElementRectangleZones)
					DesignerCanvas.Create(elementRectangleZone);
				foreach (var elementPolygonZone in plan.ElementPolygonZones)
					DesignerCanvas.Create(elementPolygonZone);
				foreach (var elementSubPlan in plan.ElementSubPlans)
					DesignerCanvas.Create(elementSubPlan);
				foreach (var elementDevice in plan.ElementDevices)
					DesignerCanvas.Create(elementDevice);
				DesignerCanvas.DeselectAll();
				OnUpdated();
				UpdateDeviceInZones();
			}
		}

		public void Save()
		{
			if (Plan == null)
				return;

			NormalizeZIndex();
			Plan.ClearElements();

			foreach (var designerItem in DesignerCanvas.Items)
			{
				designerItem.UpdateElementProperties();
				ElementBase elementBase = designerItem.Element;
				IPrimitive primitive = elementBase as IPrimitive;
				if (primitive != null)
					switch (primitive.Primitive)
					{
						case Primitive.Ellipse:
							Plan.ElementEllipses.Add(elementBase as ElementEllipse);
							break;
						case Primitive.Polygon:
							Plan.ElementPolygons.Add(elementBase as ElementPolygon);
							break;
						case Primitive.PolygonZone:
							Plan.ElementPolygonZones.Add(elementBase as ElementPolygonZone);
							break;
						case Primitive.Polyline:
							Plan.ElementPolylines.Add(elementBase as ElementPolyline);
							break;
						case Primitive.Rectangle:
							Plan.ElementRectangles.Add(elementBase as ElementRectangle);
							break;
						case Primitive.RectangleZone:
							Plan.ElementRectangleZones.Add(elementBase as ElementRectangleZone);
							break;
						case Primitive.SubPlan:
							Plan.ElementSubPlans.Add(elementBase as ElementSubPlan);
							break;
						case Primitive.TextBlock:
							Plan.ElementTextBlocks.Add(elementBase as ElementTextBlock);
							break;
					}
				//if (elementBase is ElementDevice)
				//{
				//    ElementDevice elementDevice = elementBase as ElementDevice;
				//    Plan.ElementDevices.Add(elementDevice);
				//}
			}
		}

		private void OnUpdated()
		{
			if (Updated != null)
				Updated(this, EventArgs.Empty);
		}
	}
}