using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class NamedTimeIntervalViewModel : BaseViewModel
	{
		public NamedSKDTimeInterval NamedTimeInterval { get; private set; }

		public NamedTimeIntervalViewModel(NamedSKDTimeInterval namedTimeInterval)
		{
			NamedTimeInterval = namedTimeInterval;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			TimeIntervals = new ObservableCollection<TimeIntervalViewModel>();
			foreach (var timeInterval in namedTimeInterval.TimeIntervals)
			{
				var timeIntervalViewModel = new TimeIntervalViewModel(timeInterval);
				TimeIntervals.Add(timeIntervalViewModel);
			}
		}

		public ObservableCollection<TimeIntervalViewModel> TimeIntervals { get; private set; }

		TimeIntervalViewModel _selectedTimeInterval;
		public TimeIntervalViewModel SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				_selectedTimeInterval = value;
				OnPropertyChanged("SelectedTimeInterval");
			}
		}

		public void Update()
		{
			OnPropertyChanged("NamedTimeInterval");
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var timeInterval = new SKDTimeInterval();
			NamedTimeInterval.TimeIntervals.Add(timeInterval);
			var timeIntervalViewModel = new TimeIntervalViewModel(timeInterval);
			TimeIntervals.Add(timeIntervalViewModel);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanAdd()
		{
			return TimeIntervals.Count < 4;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			NamedTimeInterval.TimeIntervals.Remove(SelectedTimeInterval.TimeInterval);
			TimeIntervals.Remove(SelectedTimeInterval);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanRemove()
		{
			return SelectedTimeInterval != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var namedTimeInrervalDetailsViewModel = new TimeIntervalDetailsViewModel(SelectedTimeInterval.TimeInterval);
			if (DialogService.ShowModalWindow(namedTimeInrervalDetailsViewModel))
			{
				SelectedTimeInterval.TimeInterval = SelectedTimeInterval.TimeInterval;
				SelectedTimeInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedTimeInterval != null;
		}
	}
}