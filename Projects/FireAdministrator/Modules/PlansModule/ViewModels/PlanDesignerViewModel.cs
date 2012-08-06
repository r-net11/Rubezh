using System;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using PlansModule.Designer;

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
					foreach (var elementSubPlan in plan.ElementSubPlans)
						DesignerCanvas.Create(elementSubPlan);
					foreach (var element in DesignerCanvas.Toolbox.PlansViewModel.LoadPlan(plan))
						DesignerCanvas.Create(element);
					DesignerCanvas.DeselectAll();
				}
				OnUpdated();
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