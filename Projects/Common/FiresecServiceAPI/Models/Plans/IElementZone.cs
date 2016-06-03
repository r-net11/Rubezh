using StrazhAPI.Plans.Interfaces;
using System;
using System.ComponentModel;

namespace StrazhAPI.Plans.Elements
{
	public interface IElementZone : IElementReference
	{
		Guid ZoneUID { get; set; }

		Color BackgroundColor { get; set; }

		void SetZLayer(int zlayer);

		bool IsHiddenZone { get; set; }

		int ZIndex { get; set; }

		int ZLayer { get; }

		ElementZoneType ElementZoneType { get; set; }
	}

	public enum ElementZoneType
	{
		[Description("СКД зона")]
		SKD,
	}
}