using Microsoft.Practices.Prism.Events;

namespace Infrustructure.Plans.Events
{
	public class RegisterPlanPresenterEvent<TPlan, TState> : CompositePresentationEvent<IPlanPresenter<TPlan, TState>>
	{
	}
}
