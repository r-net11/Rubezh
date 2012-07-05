using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class DefaultPainter : IPainter
	{
		#region IPainter Members

		public FrameworkElement Draw(ElementBase element)
		{
			return null;
		}

		#endregion
	}
}
