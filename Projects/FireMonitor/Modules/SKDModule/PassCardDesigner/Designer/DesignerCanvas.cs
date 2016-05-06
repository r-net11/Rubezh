using StrazhAPI.Models;
using StrazhAPI.SKD;
using Infrastructure;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using SKDModule.PassCardDesigner.ViewModels;

namespace SKDModule.PassCardDesigner.Designer
{
	public class DesignerCanvas : Infrastructure.Designer.DesignerCanvas
	{
		public PassCardTemplate PassCardTemplate { get; private set; }

		public DesignerCanvas(PassCardDesignerViewModel passCardDesignerViewModel)
			: base(passCardDesignerViewModel)
		{
		}

		public void Initialize(PassCardTemplate passCardTemplate)
		{
			PassCardTemplate = passCardTemplate;
			Initialize();
		}

		public override void Update()
		{
			Update(PassCardTemplate);
			base.Update();
		}

		protected override DesignerItem AddElement(ElementBase elementBase)
		{
			if (elementBase is ElementPassCardImageProperty)
				PassCardTemplate.ElementImageProperties.Add(elementBase as ElementPassCardImageProperty);
			else if (elementBase is ElementPassCardTextProperty)
				PassCardTemplate.ElementTextProperties.Add(elementBase as ElementPassCardTextProperty);
			else if (elementBase is ElementRectangle)
				PassCardTemplate.ElementRectangles.Add(elementBase as ElementRectangle);
			else if (elementBase is ElementEllipse)
				PassCardTemplate.ElementEllipses.Add(elementBase as ElementEllipse);
			else if (elementBase is ElementPolygon)
				PassCardTemplate.ElementPolygons.Add(elementBase as ElementPolygon);
			else if (elementBase is ElementPolyline)
				PassCardTemplate.ElementPolylines.Add(elementBase as ElementPolyline);
			else if (elementBase is ElementTextBlock)
				PassCardTemplate.ElementTextBlocks.Add(elementBase as ElementTextBlock);
			return Create(elementBase);
		}
		protected override void RemoveElement(ElementBase elementBase)
		{
			if (elementBase is ElementPassCardImageProperty)
				PassCardTemplate.ElementImageProperties.Remove(elementBase as ElementPassCardImageProperty);
			else if (elementBase is ElementPassCardTextProperty)
				PassCardTemplate.ElementTextProperties.Remove(elementBase as ElementPassCardTextProperty);
			else if (elementBase is ElementRectangle)
				PassCardTemplate.ElementRectangles.Remove(elementBase as ElementRectangle);
			else if (elementBase is ElementEllipse)
				PassCardTemplate.ElementEllipses.Remove(elementBase as ElementEllipse);
			else if (elementBase is ElementPolygon)
				PassCardTemplate.ElementPolygons.Remove(elementBase as ElementPolygon);
			else if (elementBase is ElementPolyline)
				PassCardTemplate.ElementPolylines.Remove(elementBase as ElementPolyline);
			else if (elementBase is ElementTextBlock)
				PassCardTemplate.ElementTextBlocks.Remove(elementBase as ElementTextBlock);
		}

		public override void DesignerChanged()
		{
			base.DesignerChanged();
			//ServiceFactory.SaveService.SKDPassCardLibraryChanged = true;
		}
	}
}
