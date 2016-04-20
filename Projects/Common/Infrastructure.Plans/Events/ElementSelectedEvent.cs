using RubezhAPI.Plans.Elements;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Plans.Events
{
	public class ElementSelectedEvent : CompositePresentationEvent<ElementBase>
	{
	}
}