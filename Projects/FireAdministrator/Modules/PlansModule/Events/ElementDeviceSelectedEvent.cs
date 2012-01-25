using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;

namespace PlansModule.Events
{
    public class ElementDeviceSelectedEvent : CompositePresentationEvent<Guid>
    {
    }
}
