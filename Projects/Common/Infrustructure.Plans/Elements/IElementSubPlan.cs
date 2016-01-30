using System;
using System.Windows.Media;
using Infrustructure.Plans.Interfaces;

namespace Infrustructure.Plans.Elements
{
	public interface IElementSubPlan : IElementReference
	{
		Guid PlanUID { get; set; }
		string Caption { get; set; }
		Color BackgroundColor { get; set; }
	}
}
