using Common;
using RubezhAPI.Plans.Interfaces;
using System;

namespace RubezhAPI.Plans.Elements
{
	public interface IElementMPT : IElementReference
	{
		Guid MPTUID { get; set; }
		Color BackgroundColor { get; set; }
		void SetZLayer(int zlayer);
	}
}