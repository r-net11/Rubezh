using System;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Events;
using PlansModule.Designer;
using Infrastructure.Common;

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
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(x => { UpdateDeviceInZones(); });
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(x => { UpdateDeviceInZones(); });
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(UpdateDevice);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(UpdateDevice);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(ShowDeviceProperties);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(ShowDeviceProperties);
		}

		public void Initialize(Plan plan)
		{
			Plan = plan;
			OnPropertyChanged("Plan");
			if (Plan != null)
			{
				using (new WaitWrapper())
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
					foreach (var element in DesignerCanvas.Toolbox.PlansViewModel.LoadPlan(plan))
						DesignerCanvas.Create(element);
					DesignerCanvas.DeselectAll();
				}
				OnUpdated();
				UpdateDeviceInZones();
			}
		}

		public void Save()
		{
			if (Plan == null)
				return;
			NormalizeZIndex();
		}

		private void OnUpdated()
		{
			if (Updated != null)
				Updated(this, EventArgs.Empty);
		}
	}
}