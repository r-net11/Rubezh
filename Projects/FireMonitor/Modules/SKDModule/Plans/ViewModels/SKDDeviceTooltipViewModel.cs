using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SKDDeviceTooltipViewModel : BaseViewModel
	{
		public SKDDeviceState State { get; private set; }
		public SKDDevice Device { get; private set; }

		public SKDDeviceTooltipViewModel(SKDDevice device)
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