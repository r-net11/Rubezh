using Microsoft.Practices.Prism.Events;

namespace Infrustructure.Plans.Events
{
	public class RegisterPlanExtensionEvent<T> : CompositePresentationEvent<IPlanExtension<T>>
	{
	}
}
