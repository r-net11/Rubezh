using Common;
using Infrustructure.Plans.Interfaces;
using System;
using System.ComponentModel;

namespace Infrustructure.Plans.Elements
{
	public interface IElementZone : IElementReference, IElementBase
	{
		Guid ZoneUID { get; set; }
		Color BackgroundColor { get; set; }
		void SetZLayer(int zlayer);
		int ZIndex { get; set; }
		int ZLayer { get; }
		ElementZoneType ElementZoneType { get; set; }
		bool ShowState { get; set; }
	}

	public enum ElementZoneType
	{
		[Description("Пожарная зона")]
		GK,

		[Description("Охранная зона")]
		Guard,

		[Description("СКД зона")]
		SKD,
	}
}