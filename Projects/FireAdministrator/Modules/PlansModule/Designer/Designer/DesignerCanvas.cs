using System.Windows.Controls;
using System.Windows.Input;
using StrazhAPI.Models;
using Infrastructure;
using Infrastructure.Designer;
using Infrustructure.Plans.Designer;
using StrazhAPI.Plans.Elements;
using PlansModule.ViewModels;

namespace PlansModule.Designer
{
	public class DesignerCanvas : Infrastructure.Designer.DesignerCanvas
	{
		public Plan Plan { get; private set; }

		public DesignerCanvas(PlanDesignerViewModel planDesignerViewModel)
			: base(planDesignerViewModel)
		{
			ContextMenu.Items.Add(new Separator());

			var editItem = DesignerCanvasHelper.BuildMenuItem(
				"Редактировать план", 
				"BEdit", 
				planDesignerViewModel.PlansViewModel.EditCommand
			);
			ContextMenu.Items.Add(editItem);
		}

		public void Initialize(Plan plan)
		{
			Plan = plan;
			Initialize();
		}

		public override void Update()
		{
			Update(Plan);
			base.Update();
		}

		protected override DesignerItem AddElement(ElementBase elementBase)
		{
			if (elementBase is ElementRectangle)
				Plan.ElementRectangles.Add(elementBase as ElementRectangle);
			else if (elementBase is ElementEllipse)
				Plan.ElementEllipses.Add(elementBase as ElementEllipse);
			else if (elementBase is ElementPolygon)
				Plan.ElementPolygons.Add(elementBase as ElementPolygon);
			else if (elementBase is ElementPolyline)
				Plan.ElementPolylines.Add(elementBase as ElementPolyline);
			else if (elementBase is ElementTextBlock)
				Plan.ElementTextBlocks.Add(elementBase as ElementTextBlock);
			else if (elementBase is ElementSubPlan)
				Plan.ElementSubPlans.Add(elementBase as ElementSubPlan);
			else
				((PlanDesignerViewModel)PlanDesignerViewModel).PlansViewModel.ElementAdded(elementBase);

			return Create(elementBase);
		}
		protected override void RemoveElement(ElementBase elementBase)
		{
			if (elementBase is ElementRectangle)
				Plan.ElementRectangles.Remove(elementBase as ElementRectangle);
			else if (elementBase is ElementEllipse)
				Plan.ElementEllipses.Remove(elementBase as ElementEllipse);
			else if (elementBase is ElementPolygon)
				Plan.ElementPolygons.Remove(elementBase as ElementPolygon);
			else if (elementBase is ElementPolyline)
				Plan.ElementPolylines.Remove(elementBase as ElementPolyline);
			else if (elementBase is ElementTextBlock)
				Plan.ElementTextBlocks.Remove(elementBase as ElementTextBlock);
			else if (elementBase is ElementSubPlan)
				Plan.ElementSubPlans.Remove(elementBase as ElementSubPlan);
			else
				((PlanDesignerViewModel)PlanDesignerViewModel).PlansViewModel.ElementRemoved(elementBase);
		}

		public override void DesignerChanged()
		{
			base.DesignerChanged();
			ServiceFactory.SaveService.PlansChanged = true;
			CommandManager.InvalidateRequerySuggested();
		}
	}
}