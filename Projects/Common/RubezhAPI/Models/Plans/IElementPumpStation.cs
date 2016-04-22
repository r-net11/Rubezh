using Common;
using RubezhAPI.Plans.Interfaces;
using System;

namespace RubezhAPI.Plans.Elements
{
	/// <summary>
	/// Defines an Interface of a Pump Station Element.
	/// </summary>
	public interface IElementPumpStation : IElementReference
	{
		Guid PumpStationUID { get; set; }
		Color BackgroundColor { get; set; }
		void SetZLayer(int zlayer);
		bool ShowState { get; set; }
		bool ShowDelay { get; set; }
	}
}
