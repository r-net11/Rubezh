using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;

namespace Infrastructure.Events
{
	public class SKDReportOrganisationChangedEvent : CompositePresentationEvent<List<Guid>>
	{
	}
}