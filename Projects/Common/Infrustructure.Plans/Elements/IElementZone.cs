using System;
using System.Windows.Media;
using System.ComponentModel;
using Infrustructure.Plans.Interfaces;

namespace Infrustructure.Plans.Elements
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
		[Description("Пожарная зона")]
		GK,

		[Description("Охранная зона")]
		Guard,

		[Description("СКД зона")]
		SKD,
	}
}