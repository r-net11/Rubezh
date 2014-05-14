using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class EventNameViewModel : BaseViewModel
	{
		public EventNameViewModel(EventName xEvent)
		{
			EventName = xEvent;
		}

		public EventName EventName { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}
}
