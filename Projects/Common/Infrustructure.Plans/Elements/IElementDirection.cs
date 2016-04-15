using Common;
using Infrustructure.Plans.Interfaces;
using System;

namespace Infrustructure.Plans.Elements
{
	public interface IElementDirection : IElementReference
	{
		Guid DirectionUID { get; set; }
		Color BackgroundColor { get; set; }
		void SetZLayer(int zlayer);
		bool ShowState { get; set; }
		bool ShowDelay { get; set; }
	}
}