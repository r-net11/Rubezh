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
				designerItem.SavePropertiesToElementBase();
				ElementBase elementBase = designerItem.ElementBase;

				if (elementBase is ElementBasePolygon)
				{
					ElementBasePolygon elementPolygon = elementBase as ElementBasePolygon;
					if (designerItem.Content != null)
						elementPolygon.Points = new System.Windows.Media.PointCollection((designerItem.Content as Polygon).Points);
				}

				if (elementBase is ElementRectangle)
				{
					ElementRectangle elementRectangle = elementBase as ElementRectangle;
					Plan.ElementRectangles.Add(elementRectangle);
				}
				if (elementBase is ElementEllipse)
				{
					ElementEllipse elementEllipse = elementBase as ElementEllipse;
					Plan.ElementEllipses.Add(elementEllipse);
				}
				if (elementBase is ElementTextBlock)
				{
					ElementTextBlock elementTextBlock = elementBase as ElementTextBlock;
					Plan.ElementTextBlocks.Add(elementTextBlock);
				}
				if (elementBase is ElementPolygon)
				{
					ElementPolygon elementPolygon = elementBase as ElementPolygon;
					Plan.ElementPolygons.Add(elementPolygon);
				}
				if (elementBase is ElementPolyline)
				{
					ElementPolyline elementPolyline = elementBase as ElementPolyline;
					Plan.ElementPolylines.Add(elementPolyline);
				}
				if (elementBase is ElementPolygonZone)
				{
					ElementPolygonZone elementPolygonZone = elementBase as ElementPolygonZone;
					Plan.ElementPolygonZones.Add(elementPolygonZone);
				}
				if (elementBase is ElementRectangleZone)
				{
					ElementRectangleZone elementRectangleZone = elementBase as ElementRectangleZone;
					Plan.ElementRectangleZones.Add(elementRectangleZone);
				}
				if (elementBase is ElementDevice)
				{
					ElementDevice elementDevice = elementBase as ElementDevice;
					Plan.ElementDevices.Add(elementDevice);
				}
				if (elementBase is ElementSubPlan)
				{
					ElementSubPlan elementSubPlan = elementBase as ElementSubPlan;
					Plan.ElementSubPlans.Add(elementSubPlan);
				}
			}
		}

		private void OnUpdated()
		{
			if (Updated != null)
				Updated(this, EventArgs.Empty);
		}
	}
}