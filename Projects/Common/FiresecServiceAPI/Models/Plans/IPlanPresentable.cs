using System;
using System.Collections.Generic;

namespace StrazhAPI.Plans.Interfaces
{
	public interface IPlanPresentable : IChangedNotification
	{
		List<Guid> PlanElementUIDs { get; set; }
	}
}