using Microsoft.Practices.Prism.Events;
using RubezhAPI.SKD;
using System.Collections.Generic;

namespace Infrastructure.Events
{
	public class SkdReportOrganisationsListChangedEvent : CompositePresentationEvent<IEnumerable<Organisation>>
	{
	}
}