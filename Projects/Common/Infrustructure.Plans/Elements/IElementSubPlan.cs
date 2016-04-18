using Common;
using Infrustructure.Plans.Interfaces;
using System;

namespace Infrustructure.Plans.Elements
{
	public interface IElementSubPlan : IElementReference
	{
		Guid PlanUID { get; set; }
		string Caption { get; set; }
		Color BackgroundColor { get; set; }
	}
}
