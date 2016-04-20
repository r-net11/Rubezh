using RubezhAPI.Models;
using Infrastructure.Designer.DesignerItems;
using RubezhAPI.Plans.Elements;
using PlansModule.ViewModels;

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemSubPlan : DesignerItemRectangle
	{
		public DesignerItemSubPlan(ElementBase element)
			: base(element)
		{
			if (Element is IElementSubPlan)
			{
				Title = Helper.GetSubPlanTitle((IElementSubPlan)Element);
				Group = Helper.SubPlanAlias;
			}
		}
		protected override Infrastructure.Common.Windows.Windows.ViewModels.SaveCancelDialogViewModel CreatePropertiesViewModel()
		{
			if (Element is IElementSubPlan)
				return new SubPlanPropertiesViewModel(Element as IElementSubPlan);
			return base.CreatePropertiesViewModel();
		}
		public override void UpdateElementProperties()
		{
			if (Element is IElementSubPlan)
				Title = Helper.GetSubPlanTitle((IElementSubPlan)Element);
			base.UpdateElementProperties();
		}
		protected override void SetIsMouseOver(bool value)
		{
			if (Element is IElementSubPlan)
				Title = Helper.GetSubPlanTitle((IElementSubPlan)Element);
			base.SetIsMouseOver(value);
		}
	}
}