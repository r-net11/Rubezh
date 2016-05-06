using StrazhAPI.Models;
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
			if (Element is ElementSubPlan)
			{
				Title = Helper.GetSubPlanTitle((ElementSubPlan)Element);
				Group = Helper.SubPlanAlias;
			}
		}
		protected override Infrastructure.Common.Windows.ViewModels.SaveCancelDialogViewModel CreatePropertiesViewModel()
		{
			if (Element is ElementSubPlan)
				return new SubPlanPropertiesViewModel(Element as ElementSubPlan);
			return base.CreatePropertiesViewModel();
		}
		public override void UpdateElementProperties()
		{
			if (Element is ElementSubPlan)
				Title = Helper.GetSubPlanTitle((ElementSubPlan)Element);
			base.UpdateElementProperties();
		}
		protected override void SetIsMouseOver(bool value)
		{
			if (Element is ElementSubPlan)
				Title = Helper.GetSubPlanTitle((ElementSubPlan)Element);
			base.SetIsMouseOver(value);
		}
	}
}