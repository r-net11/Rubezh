using System.Collections.ObjectModel;
using FiresecAPI.GK;
using Infrastructure.Client.Plans.Presenter;

namespace GKModule.ViewModels
{
	public class DeviceTooltipViewModel : StateTooltipViewModel<GKDevice>
	{
		public ObservableCollection<XStateClassViewModel> StateClasses { get; private set; }
		public GKDevice Device
		{
			get { return Item; }
		}
		private GKState _state;

		public DeviceTooltipViewModel(GKDevice device)
			: base(device)
		{
			StateClasses = new ObservableCollection<XStateClassViewModel>();
			_state = device.State;
		}

		public override void OnStateChanged()
		{
			base.OnStateChanged();
			StateClasses.Clear();
			foreach (var stateClass in _state.StateClasses)
				StateClasses.Add(new XStateClassViewModel(_state.Device, stateClass));
		}
	}
}