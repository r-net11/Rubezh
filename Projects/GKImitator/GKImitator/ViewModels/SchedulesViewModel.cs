using FiresecAPI.GK;
using GKImitator.Processor;
using GKProcessor;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GKImitator.ViewModels
{
	public class SchedulesViewModel : DialogViewModel
	{
		public SchedulesViewModel()
		{
			Title = "Графики работ прибора";
			Schedules = new ObservableCollection<ImitatorSchedule>(DBHelper.ImitatorSerializedCollection.ImitatorSchedules);
		}

		public ObservableCollection<ImitatorSchedule> Schedules { get; private set; }

		ImitatorSchedule _selectedSchedule;
		public ImitatorSchedule SelectedSchedule
		{
			get { return _selectedSchedule; }
			set
			{
				_selectedSchedule = value;
				OnPropertyChanged(() => SelectedSchedule);
			}
		}
	}
}