using System;
using System.Collections.Generic;

namespace Infrustructure.Plans.Interfaces
{
	public interface IPlanPresentable : IChangedNotification
	{
		List<Guid> PlanElementUIDs { get; set; }
	}
}