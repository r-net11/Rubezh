using System;
using Microsoft.Practices.Prism.Events;
using System.Collections.Generic;
using FiresecAPI.Models;

namespace Infrastructure.Events
{
	public class NotifyEvent : CompositePresentationEvent<string>
	{
	}
}