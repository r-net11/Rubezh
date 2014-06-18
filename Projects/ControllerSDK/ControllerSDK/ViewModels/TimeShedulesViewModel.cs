using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChinaSKDDriver;
using ChinaSKDDriverAPI;
using ControllerSDK.Views;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

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
			var timeShedules = Wrapper.GetTimeShedules(MainWindow.LoginID);

			foreach (var timeShedule in timeShedules)
			{
				Trace.WriteLine(timeShedule.Mask + " " + timeShedule.BeginHours + " " + timeShedule.BeginMinutes + " " + timeShedule.BeginSeconds + " " + timeShedule.EndHours + " " + timeShedule.EndMinutes + " " + timeShedule.EndSeconds);
			}

			TimeShedules = new ObservableRangeCollection<TimeSheduleViewModel>();
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					var timeShedule = timeShedules[i * 4 + j];
					var timeSheduleViewModel = new TimeSheduleViewModel(timeShedule);
					timeSheduleViewModel.DayNo = i;
					timeSheduleViewModel.DoorNo = j;
					TimeShedules.Add(timeSheduleViewModel);
				}
			}
			SelectedTimeShedule = TimeShedules.FirstOrDefault();
		}

		public RelayCommand SetTimeShedulesCommand { get; private set; }
		void OnSetTimeShedules()
		{
			var timeShedules = new List<TimeShedule>();
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					var timeShedule = new TimeShedule();
					timeShedule.BeginHours = j + 4;
					timeShedule.EndHours = i + 10;
					timeShedule.EndMinutes = 10 + j * 10;
					timeShedules.Add(timeShedule);
				}
			}
			var result = Wrapper.SetTimeShedules(MainWindow.LoginID, timeShedules);
		}

		ObservableRangeCollection<TimeSheduleViewModel> _timeShedules;
		public ObservableRangeCollection<TimeSheduleViewModel> TimeShedules
		{
			get { return _timeShedules; }
			set
			{
				_timeShedules = value;
				OnPropertyChanged("TimeShedules");
			}
		}

		TimeSheduleViewModel _selectedTimeShedule;
		public TimeSheduleViewModel SelectedTimeShedule
		{
			get { return _selectedTimeShedule; }
			set
			{
				_selectedTimeShedule = value;
				OnPropertyChanged("SelectedTimeShedule");
			}
		}
	}
}