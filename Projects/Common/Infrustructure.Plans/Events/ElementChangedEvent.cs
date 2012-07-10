using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Events
{
    public class ElementChangedEvent : CompositePresentationEvent<List<ElementBase>>
    {
    }
}
