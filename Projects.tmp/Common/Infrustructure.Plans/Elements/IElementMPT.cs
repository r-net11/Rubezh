using System;
using System.Windows.Media;
using Infrustructure.Plans.Interfaces;

namespace Infrustructure.Plans.Elements
{
	public interface IElementMPT : IElementReference
	{
		Guid MPTUID { get; set; }
		Color BackgroundColor { get; set; }
		void SetZLayer(int zlayer);
	}
}