using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Common;

namespace AutomationModule.ViewModels
{
	public class SchedulesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public SchedulesViewModel()
		{
			Menu = new SchedulesMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
		}

		public void Initialize()
		{
			Schedules = new SortableObservableCollection<ScheduleViewModel>();
			if (FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSchedules == null)
				FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSchedules = new List<AutomationSchedule>();
			foreach (var schedule in FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSchedules)
			{
				var scheduleViewModel = new ScheduleViewModel(schedule);
				Schedules.Add(scheduleViewModel);
			}
			SelectedSchedule = Schedules.FirstOrDefault();
			OnPropertyChanged(()=>Schedules);
		}


		public SortableObservableCollection<ScheduleViewModel> Schedules { get; private set; }

		ScheduleViewModel _selectedSchedule;
		public ScheduleViewModel SelectedSchedule
		{
			get { return _selectedSchedule; }
			set
			{
				_selectedSchedule = value;
				OnPropertyChanged(() => SelectedSchedule);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var scheduleDetailsViewModel = new ScheduleDetailsViewModel();
			if (DialogService.ShowModalWindow(scheduleDetailsViewModel))
			{
				FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSchedules.Add(scheduleDetailsViewModel.Schedule);
				var scheduleViewModel = new ScheduleViewModel(scheduleDetailsViewModel.Schedule);
				Schedules.Add(scheduleViewModel);
				SelectedSchedule = scheduleViewModel;
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		bool CanAdd()
		{
			return true;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var index = Schedules.IndexOf(SelectedSchedule);
			FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSchedules.Remove(SelectedSchedule.Schedule);
			Schedules.Remove(SelectedSchedule);
			index = Math.Min(index, Schedules.Count - 1);
			if (index > -1)
				SelectedSchedule = Schedules[index];
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var scheduleDetailsViewModel = new ScheduleDetailsViewModel(SelectedSchedule.Schedule);
			if (DialogService.ShowModalWindow(scheduleDetailsViewModel))
			{
				SelectedSchedule.Schedule = scheduleDetailsViewModel.Schedule;
				SelectedSchedule.Update();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		bool CanEditDelete()
		{
			return SelectedSchedule != null;
		}

		public void Select(Guid scheduleUid)
		{
			if (scheduleUid != Guid.Empty)
			{
				SelectedSchedule = Schedules.FirstOrDefault(item => item.Schedule.Uid == scheduleUid);
			}
		}

		public override void OnShow()
		{
			if (SelectedSchedule != null)
				SelectedSchedule.UpdateContent();

			if (Schedules != null)
				Schedules.Sort(x => x.Name);

			base.OnShow();
		}
	}
}