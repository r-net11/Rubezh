using Microsoft.Practices.Prism.Events;

namespace Infrustructure.Plans.Events
{
	public class RegisterPlanPresenterEvent<T> : CompositePresentationEvent<IPlanPresenter<T>>
	{
	}
}
