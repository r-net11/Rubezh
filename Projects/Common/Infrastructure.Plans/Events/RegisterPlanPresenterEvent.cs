using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Plans.Events
{
	public class RegisterPlanPresenterEvent<TPlan, TState> : CompositePresentationEvent<IPlanPresenter<TPlan, TState>>
	{
	}
}