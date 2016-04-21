using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Plans.Events
{
	public class RegisterPlanExtensionEvent<T> : CompositePresentationEvent<IPlanExtension<T>>
	{
	}
}