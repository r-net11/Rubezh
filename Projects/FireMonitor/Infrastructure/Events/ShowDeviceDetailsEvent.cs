using System;
using Microsoft.Practices.Prism.Events;
using FiresecAPI.Models;
using Infrastructure.Models;

namespace Infrastructure.Events
{
	public class ShowDeviceDetailsEvent : CompositePresentationEvent<ElementDeviceReference>
    {
    }
}