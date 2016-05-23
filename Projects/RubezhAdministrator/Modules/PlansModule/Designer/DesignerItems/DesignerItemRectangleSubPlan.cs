using Infrastructure.Plans.DesignerItems;
using PlansModule.ViewModels;
using RubezhAPI.Plans.Elements;

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemRectangleSubPlan : DesignerItemRectangle
	{
		public DesignerItemRectangleSubPlan(ElementBase element)
			: base(element)
		{

			Title = Helper.GetSubPlanTitle((IElementSubPlan)Element);
			Group = Helper.SubPlanAlias;
			IconSource = "/Controls;component/Images/CMap.png";
		}
		protected override Infrastructure.Common.Windows.ViewModels.SaveCancelDialogViewModel CreatePropertiesViewModel()
		{
			return new SubPlanPropertiesViewModel((IElementSubPlan)Element, DesignerCanvas);

		}
		public override void UpdateElementProperties()
		{
			Title = Helper.GetSubPlanTitle((IElementSubPlan)Element);
			base.UpdateElementProperties();
		}
		protected override void SetIsMouseOver(bool value)
		{
			Title = Helper.GetSubPlanTitle((IElementSubPlan)Element);
			base.SetIsMouseOver(value);
		}
	}
}