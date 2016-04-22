using Common;
using RubezhAPI.Plans.Interfaces;
using System;

namespace RubezhAPI.Plans.Elements
{
	/// <summary>
	/// Defines an Interface of a Delay Element.
	/// </summary>
	public interface IElementDelay : IElementReference
	{
		Guid DelayUID { get; set; }
		Color BackgroundColor { get; set; }
		void SetZLayer(int zlayer);
		bool ShowState { get; set; }
		bool ShowDelay { get; set; }
	}
}
