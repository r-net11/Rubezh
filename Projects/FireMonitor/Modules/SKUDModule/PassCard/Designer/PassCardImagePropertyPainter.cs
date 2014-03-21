using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Designer;
using FiresecAPI.SKD.PassCardLibrary;

namespace SKDModule.PassCard.Designer
{
	class PassCardImagePropertyPainter : RectanglePainter
	{
		public PassCardImagePropertyPainter(CommonDesignerCanvas designerCanvas, ElementPassCardImageProperty element)
			: base(designerCanvas, element)
		{
		}
	}
}