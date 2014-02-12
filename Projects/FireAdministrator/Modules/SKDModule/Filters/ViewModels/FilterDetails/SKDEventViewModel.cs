using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace SKDModule.ViewModels
{
	public class SKDEventNameViewModel : BaseViewModel
	{
		public SKDEventNameViewModel(string name)
		{
			Name = name;
			StateClass = XStateClass.Info;
		}

		public string Name { get; private set; }
		public string Description { get; private set; }
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