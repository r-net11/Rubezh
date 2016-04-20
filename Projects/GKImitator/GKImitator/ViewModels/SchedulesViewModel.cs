using RubezhAPI.GK;
using GKImitator.Processor;
using GKProcessor;
using Infrastructure.Common.Windows.Windows.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using RubezhDAL.DataClasses;

namespace GKImitator.ViewModels
{
	public class SchedulesViewModel : DialogViewModel
	{
		public SchedulesViewModel()
		{
			Title = "Графики работ прибора";

			using (var dbService = new DbService())
			{
				var schedules = dbService.ImitatorScheduleTranslator.Get();
				schedules.ForEach(x => x.ImitatorSheduleIntervals = x.ImitatorSheduleIntervals.OrderBy(y => y.StartSeconds).ToList());
				Schedules = new ObservableCollection<ImitatorSchedule>(schedules);
			}
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