using System;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class SKDReportOrganisationChangedEvent : CompositePresentationEvent<List<Guid>>
	{
	}
}
