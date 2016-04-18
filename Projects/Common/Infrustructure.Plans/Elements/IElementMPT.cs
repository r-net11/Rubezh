using Common;
using Infrustructure.Plans.Interfaces;
using System;

namespace Infrustructure.Plans.Elements
{
	public interface IElementMPT : IElementReference
	{
		Guid MPTUID { get; set; }
		Color BackgroundColor { get; set; }
		void SetZLayer(int zlayer);
	}
}