using System.Collections.ObjectModel;
using StrazhAPI.SKD;
using Infrastructure.Client.Plans.Presenter;

namespace StrazhModule.ViewModels
{
	public class SKDDeviceTooltipViewModel : StateTooltipViewModel<SKDDevice>
	{
		public ObservableCollection<XStateClassViewModel> StateClasses { get; private set; }
		public SKDDevice Device
		{
			get { return Item; }
		}
		private SKDDeviceState _state;

		public SKDDeviceTooltipViewModel(SKDDevice device)
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