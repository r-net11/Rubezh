using Common;
using RubezhAPI.Plans.Interfaces;
using System;

namespace RubezhAPI.Plans.Elements
{
	public interface IElementSubPlan : IElementReference
	{
		Guid PlanUID { get; set; }
		string Caption { get; set; }
		Color BackgroundColor { get; set; }
	}
}
