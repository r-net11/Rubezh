using System;
using System.Collections.Generic;

namespace RubezhAPI.Plans.Interfaces
{
	public interface IPlanPresentable : IChangedNotification
	{
		List<Guid> PlanElementUIDs { get; set; }
		void OnPlanElementUIDsChanged();
	}
}