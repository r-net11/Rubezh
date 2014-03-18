using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace SKDModule.ViewModels
{
	public class OrganisationSlideDayIntervalsViewModel : ViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public Organization Organization { get; private set; }
		EmployeeSlideDayInterval IntervalToCopy;

		public OrganisationSlideDayIntervalsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
		}

		public void Initialize(Organization organization, List<EmployeeSlideDayInterval> employeeSlideDayIntervals)
		{
			Organization = organization;

			var neverTimeInterval = employeeSlideDayIntervals.FirstOrDefault(x => x.Name == "Никогда" && x.IsDefault);
			if (neverTimeInterval == null)
			{
				neverTimeInterval = new EmployeeSlideDayInterval() { Name = "Никогда", IsDefault = true };
				//neverTimeInterval.TimeIntervalUIDs.Add(Guid.Empty);
				employeeSlideDayIntervals.Add(neverTimeInterval);
			}

			SlideDayIntervals = new ObservableCollection<SlideDayIntervalViewModel>();
			foreach (var slideDayInterval in employeeSlideDayIntervals)
			{
				var timeInrervalViewModel = new SlideDayIntervalViewModel(slideDayInterval);
				SlideDayIntervals.Add(timeInrervalViewModel);
			}
			SelectedSlideDayInterval = SlideDayIntervals.FirstOrDefault();
		}

		ObservableCollection<SlideDayIntervalViewModel> _slideDayIntervals;
		public ObservableCollection<SlideDayIntervalViewModel> SlideDayIntervals
		{
			get { return _slideDayIntervals; }
			set
			{
				_slideDayIntervals = value;
				OnPropertyChanged("SlideDayIntervals");
			}
		}

		SlideDayIntervalViewModel _selectedSlideDayInterval;
		public SlideDayIntervalViewModel SelectedSlideDayInterval
		{
			get { return _selectedSlideDayInterval; }
			set
			{
				_selectedSlideDayInterval = value;
				OnPropertyChanged("SelectedSlideDayInterval");
			}
		}

		public void Select(Guid intervalUID)
		{
			if (intervalUID != Guid.Empty)
			{
				var intervalViewModel = SlideDayIntervals.FirstOrDefault(x => x.SlideDayInterval.UID == intervalUID);
				if (intervalViewModel != null)
				{
					SelectedSlideDayInterval = intervalViewModel;
				}
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var slideDayIntervalDetailsViewModel = new SlideDayIntervalDetailsViewModel(this);
			if (DialogService.ShowModalWindow(slideDayIntervalDetailsViewModel))
			{
				var timeInrervalViewModel = new SlideDayIntervalViewModel(slideDayIntervalDetailsViewModel.SlideDayInterval);
				SlideDayIntervals.Add(timeInrervalViewModel);
				SelectedSlideDayInterval = timeInrervalViewModel;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			SlideDayIntervals.Remove(SelectedSlideDayInterval);
		}
		bool CanDelete()
		{
			return SelectedSlideDayInterval != null && !SelectedSlideDayInterval.SlideDayInterval.IsDefault;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var slideDayIntervalDetailsViewModel = new SlideDayIntervalDetailsViewModel(this, SelectedSlideDayInterval.SlideDayInterval);
			if (DialogService.ShowModalWindow(slideDayIntervalDetailsViewModel))
			{
				SelectedSlideDayInterval.Update();
			}
		}
		bool CanEdit()
		{
			return SelectedSlideDayInterval != null && !SelectedSlideDayInterval.SlideDayInterval.IsDefault;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			IntervalToCopy = CopyInterval(SelectedSlideDayInterval.SlideDayInterval);
		}
		bool CanCopy()
		{
			return SelectedSlideDayInterval != null && !SelectedSlideDayInterval.SlideDayInterval.IsDefault;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var newInterval = CopyInterval(IntervalToCopy);
			var timeInrervalViewModel = new SlideDayIntervalViewModel(newInterval);
			SlideDayIntervals.Add(timeInrervalViewModel);
			SelectedSlideDayInterval = timeInrervalViewModel;
		}
		bool CanPaste()
		{
			return IntervalToCopy != null;
		}

		EmployeeSlideDayInterval CopyInterval(EmployeeSlideDayInterval source)
		{
			var copy = new EmployeeSlideDayInterval();
			copy.Name = source.Name;
			foreach (var timeIntervalUID in source.TimeIntervalUIDs)
			{
				copy.TimeIntervalUIDs.Add(timeIntervalUID);
			}
			return copy;
		}
	}
}