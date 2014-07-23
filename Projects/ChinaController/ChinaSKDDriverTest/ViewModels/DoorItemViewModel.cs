using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class DoorItemViewModel : BaseViewModel
	{
		public DoorItemViewModel(int no)
		{
			No = no;
		}

		public int No { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}

		int _timeSheduleNo;
		public int TimeSheduleNo
		{
			get { return _timeSheduleNo; }
			set
			{
				_timeSheduleNo = value;
				OnPropertyChanged(() => TimeSheduleNo);
			}
		}
	}
}