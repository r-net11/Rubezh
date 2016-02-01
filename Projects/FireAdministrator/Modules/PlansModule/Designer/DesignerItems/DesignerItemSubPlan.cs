using RubezhAPI.Models;
using Infrastructure.Designer.DesignerItems;
using Infrustructure.Plans.Elements;
using PlansModule.ViewModels;

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemSubPlan : DesignerItemRectangle
	{
		public DesignerItemSubPlan(ElementBase element)
			: base(element)
		{
			if (Element is ElementRectangleSubPlan)
			{
				Title = Helper.GetSubPlanTitle((ElementRectangleSubPlan)Element);
				Group = Helper.SubPlanAlias;
			}
		}
		protected override Infrastructure.Common.Windows.ViewModels.SaveCancelDialogViewModel CreatePropertiesViewModel()
		{
			if (Element is ElementRectangleSubPlan)
				return new SubPlanPropertiesViewModel(Element as ElementRectangleSubPlan);
			return base.CreatePropertiesViewModel();
		}
		public override void UpdateElementProperties()
		{
			if (Element is ElementRectangleSubPlan)
				Title = Helper.GetSubPlanTitle((ElementRectangleSubPlan)Element);
			base.UpdateElementProperties();
		}
		protected override void SetIsMouseOver(bool value)
		{
			if (Element is ElementRectangleSubPlan)
				Title = Helper.GetSubPlanTitle((ElementRectangleSubPlan)Element);
			base.SetIsMouseOver(value);
		}
	}
}