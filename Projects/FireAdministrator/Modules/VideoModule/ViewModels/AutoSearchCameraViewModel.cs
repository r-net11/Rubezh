using Entities.DeviceOriented;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class AutoSearchCameraViewModel:BaseViewModel
	{
		public DeviceSearchInfo DeviceSearchInfo { get; private set; }
		public AutoSearchCameraViewModel(DeviceSearchInfo deviceSearchInfo)
		{
			DeviceSearchInfo = deviceSearchInfo;
		}

		private bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(()=>IsChecked);
			}
		}

		public bool IsAdded { get; set; }
	}
}
