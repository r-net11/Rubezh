using Infrustructure.Plans.Interfaces;
using System;
using System.ComponentModel;
using System.Windows.Media;
using LocalizationConveters;

namespace Infrustructure.Plans.Elements
{
	public interface IElementZone : IElementReference, IElementBase
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
		//[Description("СКД зона")]
        [LocalizedDescription(typeof(Resources.Language.Elements.IElementZone),"SKD")]
		SKD,
	}
}