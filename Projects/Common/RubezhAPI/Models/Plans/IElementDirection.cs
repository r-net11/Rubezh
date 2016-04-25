using Common;
using RubezhAPI.Plans.Interfaces;
using System;

namespace RubezhAPI.Plans.Elements
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