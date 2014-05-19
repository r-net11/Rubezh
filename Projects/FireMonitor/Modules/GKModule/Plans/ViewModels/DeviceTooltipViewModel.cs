using System.Collections.ObjectModel;
using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DeviceTooltipViewModel : BaseViewModel
	{
		public XState State { get; private set; }
		public XDevice Device { get; private set; }

		public DeviceTooltipViewModel(XDevice device)
		{
			Device = device;
			State = device.State;
			StateClasses = new ObservableCollection<XStateClassViewModel>();
		}

		public void OnStateChanged()
		{
			OnPropertyChanged("State");
			StateClasses.Clear();
			foreach (var stateClass in State.StateClasses)
			{
				StateClasses.Add(new XStateClassViewModel(State.Device, stateClass));
			}
		}

		public ObservableCollection<XStateClassViewModel> StateClasses { get; private set; }
	}
}