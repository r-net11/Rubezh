using System.Collections.Generic;
using Common;
using FiresecAPI.SKD.PassCardLibrary;
using Infrastructure.Common;
using Infrustructure.Plans.Elements;
using SKDModule.PassCard.Designer;

namespace SKDModule.ViewModels
{
	public class PassCardDesignerViewModel : Infrastructure.Designer.ViewModels.PlanDesignerViewModel
	{
		public PassCardTemplate PassCardTemplate { get; private set; }
		public PassCardDesignerViewModel()
		{
			DesignerCanvas = new DesignerCanvas(this);
			DesignerCanvas.Toolbox.IsRightPanel = false;
			AllowScalePoint = false;
			FullScreenSize = false;
			CanCollapse = false;
		}

		public void Initialize(PassCardTemplate passCardTemplate)
		{
			PassCardTemplate = passCardTemplate;
			IsNotEmpty = PassCardTemplate != null;
			using (new TimeCounter("\tPassCardDesignerViewModel.Initialize: {0}"))
			using (new WaitWrapper())
			{
				using (new TimeCounter("\t\tDesignerCanvas.Initialize: {0}"))
					((DesignerCanvas)DesignerCanvas).Initialize(PassCardTemplate);
				if (PassCardTemplate != null)
				{
					using (new TimeCounter("\t\tDesignerItem.Create: {0}"))
					{
						foreach (var elementBase in EnumerateElements())
							DesignerCanvas.Create(elementBase);
						DesignerCanvas.UpdateZIndex();
					}
					using (new TimeCounter("\t\tPassCardDesignerViewModel.OnUpdated: {0}"))
						Update();
				}
			}
			ResetHistory();
		}
		private IEnumerable<ElementBase> EnumerateElements()
		{
			foreach (var elementTextProperty in PassCardTemplate.ElementTextProperties)
				yield return elementTextProperty;
			foreach (var elementImageProperty in PassCardTemplate.ElementImageProperties)
				yield return elementImageProperty;
			foreach (var elementRectangle in PassCardTemplate.ElementRectangles)
				yield return elementRectangle;
			foreach (var elementEllipse in PassCardTemplate.ElementEllipses)
				yield return elementEllipse;
			foreach (var elementTextBlock in PassCardTemplate.ElementTextBlocks)
				yield return elementTextBlock;
			foreach (var elementPolygon in PassCardTemplate.ElementPolygons)
				yield return elementPolygon;
			foreach (var elementPolyline in PassCardTemplate.ElementPolylines)
				yield return elementPolyline;
		}
		public override void RegisterDesignerItem(Infrustructure.Plans.Designer.DesignerItem designerItem)
		{
			base.RegisterDesignerItem(designerItem);
			if (designerItem.Element is IElementPassCardProperty)
			{
				designerItem.Group = PassCardsDesignerViewModel.PassCardPropertiesGroup;
				designerItem.Title = ((IElementPassCardProperty)designerItem.Element).PropertyType.ToString();
			}
		}
	}
}