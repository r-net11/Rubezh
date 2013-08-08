using System;
using System.Windows.Media;

namespace Infrustructure.Plans.Elements
{
	public interface IElementZone
	{
		Guid UID { get; }
		Guid ZoneUID { get; set; }
		Color BackgroundColor { get; set; }
		void SetZLayer(int zlayer);
		bool IsHiddenZone { get; set; }
		int ZIndex { get; set; }
		int ZLayer { get; }
	}
}