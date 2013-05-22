using System;
using System.Windows.Media;

namespace Infrustructure.Plans.Elements
{
	public interface IElementDirection
	{
		Guid UID { get; }
		Guid DirectionUID { get; set; }
		Color BackgroundColor { get; set; }
		void SetZLayer(int zlayer);
	}
}