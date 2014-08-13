using System;
using System.Windows.Media;

namespace Infrustructure.Plans.Elements
{
	public interface IElementBorder
	{
		Color BorderColor { get; set; }
		double BorderThickness { get; set; }
	}
}