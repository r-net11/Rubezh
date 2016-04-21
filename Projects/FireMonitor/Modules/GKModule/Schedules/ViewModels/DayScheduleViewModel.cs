using System;
using System.Linq;
using System.Collections.Generic;
using RubezhAPI;
using RubezhAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Events;
using GKModule.Events;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.ViewModels;
using RubezhClient;
using System.Collections.ObjectModel;
using RubezhClient.SKDHelpers;

namespace GKModule.ViewModels
{
	public class DayScheduleViewModel : BaseViewModel
	{
		public GKDaySchedule DaySchedule { get; private set; }

		public DayScheduleViewModel(GKDaySchedule daySchedule)
		{
			DaySchedule = daySchedule;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanEdit);

			DaySchedule = daySchedule;
			Parts = new ObservableCollection<DaySchedulePartViewModel>();
			foreach (var dayIntervalPart in daySchedule.DayScheduleParts)
			{
				var daySchedulePartViewModel = new DaySchedulePartViewModel(dayIntervalPart);
				Parts.Add(daySchedulePartViewModel);
			}

			Update(DaySchedule);
		}

		public void Update(GKDaySchedule daySchedule)
		{
			DaySchedule = daySchedule;
			OnPropertyChanged(() => DaySchedule);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
		}

		public string Name
		{
			get { return DaySchedule.Name; }
		}

		public string Description
		{
			get { return DaySchedule.Description; }
		}

		public bool ConfirmDeactivation()
		{
			return true;
			//var hasReference = GKManager.DeviceConfiguration.WeeklyIntervals.Any(item => item.WeeklyIntervalParts.Any(part => part.DayIntervalUID == DayInterval.UID));
			//return !hasReference || MessageBoxService.ShowConfirmation("Данный дневной график используется в одном или нескольких недельных графиках, Вы уверены что хотите его деактивировать?");
		}

		public ObservableCollection<DaySchedulePartViewModel> Parts { get; protected set; }

		private DaySchedulePartViewModel _selectedPart;
		public DaySchedulePartViewModel SelectedPart
		{
			get { return _selectedPart; }
			set
			{
				_selectedPart = value;
				OnPropertyChanged(() => SelectedPart);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var daySchedulePartDetailsViewModel = new DaySchedulePartDetailsViewModel();
			if (DialogService.ShowModalWindow(daySchedulePartDetailsViewModel))
			{
				DaySchedule.DayScheduleParts.Add(daySchedulePartDetailsViewModel.DaySchedulePart);
				if (GKScheduleHelper.SaveDaySchedule(DaySchedule, false))
				{
					var daySchedulePartViewModel = new DaySchedulePartViewModel(daySchedulePartDetailsViewModel.DaySchedulePart);
					Parts.Add(daySchedulePartViewModel);
					SelectedPart = daySchedulePartViewModel;
				}
			}
		}
		bool CanAdd()
		{
			return Parts.Count < 4;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			DaySchedule.DayScheduleParts.Remove(SelectedPart.DaySchedulePart);
			if (GKScheduleHelper.SaveDaySchedule(DaySchedule, false))
			{
				Parts.Remove(SelectedPart);
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var daySchedulePartDetailsViewModel = new DaySchedulePartDetailsViewModel(SelectedPart.DaySchedulePart);
			if (DialogService.ShowModalWindow(daySchedulePartDetailsViewModel) && GKScheduleHelper.SaveDaySchedule(DaySchedule, false))
			{
				SelectedPart.Update();
			}
		}
		bool CanEdit()
		{
			return SelectedPart != null;
		}
	}
}