using Microsoft.Practices.Prism.Events;
using System;

namespace Infrastructure.Events
{
    public class ShowDeviceDetailsEvent : CompositePresentationEvent<Guid>
    {
    }
}