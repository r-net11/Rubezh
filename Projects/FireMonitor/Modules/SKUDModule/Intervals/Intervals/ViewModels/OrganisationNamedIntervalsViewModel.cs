using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Organization = FiresecAPI.Organization;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class OrganisationNamedIntervalsViewModel : ViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public Organization Organization { get; private set; }
		private NamedInterval _intervalToCopy;

		public OrganisationNamedIntervalsViewModel(Organization organization)
		{
			Organization = organization;
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
		}

		public void Initialize(List<NamedInterval> namedIntervals)
		{
			NamedIntervals = new ObservableCollection<NamedIntervalViewModel>();
			foreach (var namedInterval in namedIntervals)
			{
				var timeInrervalViewModel = new NamedIntervalViewModel(namedInterval);
				NamedIntervals.Add(timeInrervalViewModel);
			}
			SelectedNamedInterval = NamedIntervals.FirstOrDefault();
		}

		private ObservableCollection<NamedIntervalViewModel> _namedIntervals;
		public ObservableCollection<NamedIntervalViewModel> NamedIntervals
		{
			get { return _namedIntervals; }
			set
			{
				_namedIntervals = value;
				OnPropertyChanged(() => NamedIntervals);
			}
		}

		private NamedIntervalViewModel _selectedNamedInterval;
		public NamedIntervalViewModel SelectedNamedInterval
		{
			get { return _selectedNamedInterval; }
			set
			{
				_selectedNamedInterval = value;
				OnPropertyChanged(() => SelectedNamedInterval);
			}
		}

		public void Select(Guid namedIntervalUID)
		{
			if (namedIntervalUID != Guid.Empty)
			{
				var namedIntervalViewModel = NamedIntervals.FirstOrDefault(x => x.NamedInterval.UID == namedIntervalUID);
				if (namedIntervalViewModel != null)
					SelectedNamedInterval = namedIntervalViewModel;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var namedIntervalDetailsViewModel = new NamedIntervalDetailsViewModel(this);
			if (DialogService.ShowModalWindow(namedIntervalDetailsViewModel) && NamedIntervalHelper.Save(namedIntervalDetailsViewModel.NamedInterval))
			{
				var namedIntervalViewModel = new NamedIntervalViewModel(namedIntervalDetailsViewModel.NamedInterval);
				NamedIntervals.Add(namedIntervalViewModel);
				SelectedNamedInterval = namedIntervalViewModel;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		private void OnDelete()
		{
			if (NamedIntervalHelper.MarkDeleted(SelectedNamedInterval.NamedInterval))
				NamedIntervals.Remove(SelectedNamedInterval);
		}
		private bool CanDelete()
		{
			return SelectedNamedInterval != null;
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var namedInrervalDetailsViewModel = new NamedIntervalDetailsViewModel(this, SelectedNamedInterval.NamedInterval);
			if (DialogService.ShowModalWindow(namedInrervalDetailsViewModel))
			{
				NamedIntervalHelper.Save(SelectedNamedInterval.NamedInterval);
				SelectedNamedInterval.Update();
			}
		}
		private bool CanEdit()
		{
			return SelectedNamedInterval != null;
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_intervalToCopy = CopyInterval(SelectedNamedInterval.NamedInterval);
		}
		private bool CanCopy()
		{
			return SelectedNamedInterval != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newInterval = CopyInterval(_intervalToCopy);
			NamedIntervalHelper.Save(newInterval);
			var timeInrervalViewModel = new NamedIntervalViewModel(newInterval);
			NamedIntervals.Add(timeInrervalViewModel);
			SelectedNamedInterval = timeInrervalViewModel;
		}
		private bool CanPaste()
		{
			return _intervalToCopy != null;
		}

		private NamedInterval CopyInterval(NamedInterval source)
		{
			var copy = new NamedInterval();
			copy.Name = source.Name;
			copy.Description = source.Description;
			copy.SlideTime = source.SlideTime;
			foreach (var timeInterval in source.TimeIntervals)
			{
				var copyNamedIntervalPart = new TimeInterval()
				{
					BeginTime = timeInterval.BeginTime,
					EndTime = timeInterval.EndTime,
					IntervalTransitionType = timeInterval.IntervalTransitionType,
				};
				copy.TimeIntervals.Add(copyNamedIntervalPart);
			}
			return copy;
		}
	}
}