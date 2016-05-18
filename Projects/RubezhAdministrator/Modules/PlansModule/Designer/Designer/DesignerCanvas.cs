using Infrastructure;
using Infrastructure.Plans;
using Infrastructure.Plans.Designer;
using PlansModule.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Input;

namespace PlansModule.Designer
{
	public class DesignerCanvas : BaseDesignerCanvas
	{
		public Plan Plan { get; private set; }

		public DesignerCanvas(PlanDesignerViewModel planDesignerViewModel)
			: base(planDesignerViewModel)
		{
			var editItem = DesignerCanvasHelper.BuildMenuItem(
				"Редактировать",
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
			else if (elementBase is ElementTextBox)
				Plan.ElementTextBoxes.Add(elementBase as ElementTextBox);
			else if (elementBase is ElementRectangleSubPlan)
				Plan.ElementRectangleSubPlans.Add(elementBase as ElementRectangleSubPlan);
			else if (elementBase is ElementPolygonSubPlan)
				Plan.ElementPolygonSubPlans.Add(elementBase as ElementPolygonSubPlan);
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
			else if (elementBase is ElementTextBox)
				Plan.ElementTextBoxes.Remove(elementBase as ElementTextBox);
			else if (elementBase is ElementRectangleSubPlan)
				Plan.ElementRectangleSubPlans.Remove(elementBase as ElementRectangleSubPlan);
			else if (elementBase is ElementPolygonSubPlan)
				Plan.ElementPolygonSubPlans.Remove(elementBase as ElementPolygonSubPlan);
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