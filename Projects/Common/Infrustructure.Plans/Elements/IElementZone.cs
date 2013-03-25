using System;
using System.Windows.Media;

namespace Infrustructure.Plans.Elements
{
	public interface IElementZone
	{
		Guid ZoneUID { get; set; }
		Color BackgroundColor { get; set; }
		void SetZLayer(int zlayer);
		bool IsHidden { get; set; }
	}
}