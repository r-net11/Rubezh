using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace SKDModule.ViewModels
{
	public class SKDEventNameViewModel : BaseViewModel
	{
		public SKDEventNameViewModel(string eventName)
		{
			EventName = eventName;
		}

		public string EventName { get; private set; }

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