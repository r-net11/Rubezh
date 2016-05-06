using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using StrazhDeviceSDK.API;

namespace ControllerSDK.ViewModels
{
	public class TimeShedulesViewModel : BaseViewModel
	{
		public TimeShedulesViewModel()
		{
			GetTimeShedulesCommand = new RelayCommand(OnGetTimeShedules);
			SetTimeShedulesCommand = new RelayCommand(OnSetTimeShedules);
		}

		public RelayCommand GetTimeShedulesCommand { get; private set; }
		void OnGetTimeShedules()
		{
			var timeShedules = MainViewModel.Wrapper.GetTimeShedules(Index);

			TimeShedules = new ObservableRangeCollection<TimeSheduleViewModel>();
			foreach (var timeShedule in timeShedules)
			{
				var timeSheduleViewModel = new TimeSheduleViewModel(timeShedule);
				TimeShedules.Add(timeSheduleViewModel);
			}
			SelectedTimeShedule = TimeShedules.FirstOrDefault();
		}

		public RelayCommand SetTimeShedulesCommand { get; private set; }
		void OnSetTimeShedules()
		{
			var timeShedules = new List<TimeShedule>();
			for (int i = 0; i < 7; i++)
			{
				var timeShedule = new TimeShedule();
				timeShedules.Add(timeShedule);
				for (int j = 0; j < 4; j++)
				{
					var timeSheduleInterval = new TimeSheduleInterval();
					timeSheduleInterval.BeginHours = 0;
					timeSheduleInterval.EndMinutes = 0;
					timeSheduleInterval.EndHours = 23;
					timeSheduleInterval.EndMinutes = 59;
					timeShedule.TimeSheduleIntervals.Add(timeSheduleInterval);
				}
			}
			var result = MainViewModel.Wrapper.SetTimeShedules(Index, timeShedules);
		}

		int _index;
		public int Index
		{
			get { return _index; }
			set
			{
				_index = value;
				OnPropertyChanged(() => Index);
			}
		}

		ObservableRangeCollection<TimeSheduleViewModel> _timeShedules;
		public ObservableRangeCollection<TimeSheduleViewModel> TimeShedules
		{
			get { return _timeShedules; }
			set
			{
				_timeShedules = value;
				OnPropertyChanged(() => TimeShedules);
			}
		}

		TimeSheduleViewModel _selectedTimeShedule;
		public TimeSheduleViewModel SelectedTimeShedule
		{
			get { return _selectedTimeShedule; }
			set
			{
				_selectedTimeShedule = value;
				OnPropertyChanged(() => SelectedTimeShedule);
			}
		}
	}
}