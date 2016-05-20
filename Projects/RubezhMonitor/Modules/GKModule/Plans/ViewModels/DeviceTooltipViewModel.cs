using Infrastructure.Plans.Presenter;
using RubezhAPI.GK;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	public class DeviceTooltipViewModel : StateTooltipViewModel<GKDevice>
	{
		public ObservableCollection<GKStateClassViewModel> StateClasses { get; private set; }
		public GKDevice Device
		{
			get { return Item; }
		}
		private GKState _state;

		public DeviceTooltipViewModel(GKDevice device)
			: base(device)
		{
			StateClasses = new ObservableCollection<GKStateClassViewModel>();
			_state = device.State;
		}

		public override void OnStateChanged()
		{
			base.OnStateChanged();
			StateClasses.Clear();
			foreach (var stateClass in _state.StateClasses)
				StateClasses.Add(new GKStateClassViewModel(_state.Device, stateClass));
		}
	}
}