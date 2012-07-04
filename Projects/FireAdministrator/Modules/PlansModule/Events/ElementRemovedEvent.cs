using System.Collections.Generic;
using FiresecAPI.Models;
using Microsoft.Practices.Prism.Events;
using Infrustructure.Plans.Elements;

namespace PlansModule.Events
{
    public class ElementRemovedEvent : CompositePresentationEvent<List<ElementBase>>
    {
    }
}
