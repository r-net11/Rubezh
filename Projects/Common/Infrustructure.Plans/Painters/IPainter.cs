using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public interface IPainter
	{
		FrameworkElement Draw(ElementBase element);
	}
}
