using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EventFilterViewModel : BaseViewModel
	{
		public EventFilterViewModel(string name, XStateClass stateClass)
		{
			Name = name;
			StateClass = stateClass;
		}

		public string Name { get; private set; }
		public XStateClass StateClass { get; private set; }

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