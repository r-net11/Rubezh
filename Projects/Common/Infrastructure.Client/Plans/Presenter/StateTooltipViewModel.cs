using Common;
using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Client.Plans.Presenter
{
	public class StateTooltipViewModel<T> : BaseViewModel
		where T : IStateProvider
	{
		public IDeviceState<XStateClass> State { get; protected set; }
		public T Item { get; protected set; }

		public StateTooltipViewModel(T item)
		{
			Item = item;
			State = item.StateClass;
		}

		public virtual void OnStateChanged()
		{
			OnPropertyChanged(() => Item);
			OnPropertyChanged(() => State);
		}
	}
}
