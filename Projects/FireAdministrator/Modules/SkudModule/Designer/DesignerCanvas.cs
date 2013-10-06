using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using Infrustructure.Plans.Designer;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using SkudModule.ViewModels;
using FiresecAPI.Models.Skud;

namespace SkudModule.Designer
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
			if (elementBase is ElementRectangle)
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
			if (elementBase is ElementRectangle)
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
			//ServiceFactory.SaveService.PlansChanged = true;
		}
	}
}
