using FiresecAPI.Models;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Services;
using PlansModule.Designer.Adorners;
using PlansModule.ViewModels;
using Infrastructure.Common.Windows.ViewModels;

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemRectangle : DesignerItemBase
	{
		public DesignerItemRectangle(ElementBase element)
			: base(element)
		{
			ResizeChrome = new ResizeChromeRectangle(this);
			if (Element is ElementRectangle)
			{
				Title = "Прямоугольник";
				Group = LayerGroupService.ElementAlias;
			}
			else if (Element is ElementEllipse)
			{
				Title = "Эллипс";
				Group = LayerGroupService.ElementAlias;
			}
			else if (Element is ElementTextBlock)
			{
				Title = "Надпись";
				Group = LayerGroupService.ElementAlias;
			}
			else if (Element is ElementSubPlan)
			{
				Title = Helper.GetSubPlanTitle((ElementSubPlan)Element);
				Group = LayerGroupService.SubPlanAlias;
			}
		}

		protected override SaveCancelDialogViewModel CreatePropertiesViewModel()
		{
			if (Element is ElementRectangle)
				return new RectanglePropertiesViewModel(Element as ElementRectangle);
			if (Element is ElementEllipse)
				return new EllipsePropertiesViewModel(Element as ElementEllipse);
			if (Element is ElementTextBlock)
				return new TextBlockPropertiesViewModel(Element as ElementTextBlock);
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
	}
}
