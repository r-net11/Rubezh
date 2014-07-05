using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using SKDModule.Intervals.Base;

namespace SKDModule.ViewModels
{
	public class TimeIntervalViewModel : BaseIntervalViewModel
	{
		public SKDTimeInterval TimeInterval { get; private set; }

		public TimeIntervalViewModel(int index, SKDTimeInterval timeInterval)
			: base(index)
		{
			TimeInterval = timeInterval;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanEdit);
			InitParts();
			IsActive = TimeInterval != null;
			Update();
		}

		public ObservableCollection<TimeIntervalPartViewModel> TimeIntervalParts { get; private set; }

		private TimeIntervalPartViewModel _selectedTimeIntervalPart;
		public TimeIntervalPartViewModel SelectedTimeIntervalPart
		{
			get { return _selectedTimeIntervalPart; }
			set
			{
				_selectedTimeIntervalPart = value;
				OnPropertyChanged(() => SelectedTimeIntervalPart);
			}
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		private string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		public override void Update()
		{
			base.Update();
			Name = IsDefault ? "Никогда" : (IsActive ? TimeInterval.Name : string.Format("Дневной график {0}", Index));
			Description = IsEnabled ? TimeInterval.Description : string.Empty;
			OnPropertyChanged(() => TimeInterval);
			OnPropertyChanged(() => TimeIntervalParts);
		}
		protected override void Activate()
		{
			if (!IsDefault)
			{
				if (IsActive && TimeInterval == null)
				{
					TimeInterval = new SKDTimeInterval()
					{
						ID = Index,
						Name = Name,
						TimeIntervalParts = new List<SKDTimeIntervalPart>(),
					};
					InitParts();
					SKDManager.TimeIntervalsConfiguration.TimeIntervals.Add(TimeInterval);
					ServiceFactory.SaveService.SKDChanged = true;
				}
				else if (!IsActive && TimeInterval != null)
				{
					SKDManager.TimeIntervalsConfiguration.TimeIntervals.Remove(TimeInterval);
					TimeInterval = null;
					InitParts();
					ServiceFactory.SaveService.SKDChanged = true;
				}
			}
			base.Activate();
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			Edit(null);
		}
		private bool CanAdd()
		{
			return TimeIntervalParts.Count < 4;
		}

		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
			TimeInterval.TimeIntervalParts.Remove(SelectedTimeIntervalPart.TimeIntervalPart);
			TimeIntervalParts.Remove(SelectedTimeIntervalPart);
			ServiceFactory.SaveService.SKDChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			Edit(SelectedTimeIntervalPart);
		}
		private bool CanEdit()
		{
			return !IsDefault && SelectedTimeIntervalPart != null;
		}

		public void Paste(SKDTimeInterval timeInterval)
		{
			IsActive = true;
			TimeInterval.TimeIntervalParts = timeInterval.TimeIntervalParts;
			InitParts();
			ServiceFactory.SaveService.SKDChanged = true;
			Update();
		}
		public void InitParts()
		{
			TimeIntervalParts = new ObservableCollection<TimeIntervalPartViewModel>();
			if (TimeInterval != null)
				foreach (var timeIntervalPart in TimeInterval.TimeIntervalParts)
				{
					var timeIntervalPartViewModel = new TimeIntervalPartViewModel(timeIntervalPart);
					TimeIntervalParts.Add(timeIntervalPartViewModel);
				}
		}

		private void Edit(TimeIntervalPartViewModel timeIntervalPartViewModel)
		{
			var timeIntervalPartDetailsViewModel = new TimeIntervalPartDetailsViewModel(timeIntervalPartViewModel == null ? null : timeIntervalPartViewModel.TimeIntervalPart);
			if (DialogService.ShowModalWindow(timeIntervalPartDetailsViewModel))
			{
				if (timeIntervalPartViewModel == null)
				{
					TimeInterval.TimeIntervalParts.Add(timeIntervalPartDetailsViewModel.TimeIntervalPart);
					timeIntervalPartViewModel = new TimeIntervalPartViewModel(timeIntervalPartDetailsViewModel.TimeIntervalPart);
					TimeIntervalParts.Add(timeIntervalPartViewModel);
					SelectedTimeIntervalPart = timeIntervalPartViewModel;
				}
				SelectedTimeIntervalPart.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
	}
}