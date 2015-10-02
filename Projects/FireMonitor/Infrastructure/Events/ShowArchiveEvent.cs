using Microsoft.Practices.Prism.Events;
using FiresecAPI.SKD;
using FiresecAPI.GK;
using FiresecAPI.Models;
using System;
using System.Collections.Generic;

namespace Infrastructure.Events
{
	public class ShowArchiveEvent : CompositePresentationEvent<List<Guid>>
	{
	}
}