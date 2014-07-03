using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class TimeIntervalViewModel : BaseViewModel
	{
		public SKDTimeInterval TimeInterval { get; private set; }
		public int Index { get; set; }

		public TimeIntervalViewModel(int index, SKDTimeInterval timeInterval)
		{
			Index = index;
			TimeInterval = timeInterval;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanEdit);
			TimeIntervalParts = new ObservableCollection<TimeIntervalPartViewModel>();
			if (timeInterval != null)
				foreach (var timeIntervalPart in timeInterval.TimeIntervalParts)
				{
					var timeIntervalPartViewModel = new TimeIntervalPartViewModel(timeIntervalPart);
					TimeIntervalParts.Add(timeIntervalPartViewModel);
				}
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

		public void Update()
		{
			Name = IsDefault ? "Никогда" : (TimeInterval == null ? string.Format("Именнованный интервал {0}", Index) : TimeInterval.Name);
			OnPropertyChanged(() => TimeInterval);
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var timeIntervalPart = new SKDTimeIntervalPart();
			TimeInterval.TimeIntervalParts.Add(timeIntervalPart);
			var timeIntervalPartViewModel = new TimeIntervalPartViewModel(timeIntervalPart);
			TimeIntervalParts.Add(timeIntervalPartViewModel);
			ServiceFactory.SaveService.SKDChanged = true;
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
			var timeIntervalPartDetailsViewModel = new TimeIntervalPartDetailsViewModel(SelectedTimeIntervalPart.TimeIntervalPart);
			if (DialogService.ShowModalWindow(timeIntervalPartDetailsViewModel))
			{
				SelectedTimeIntervalPart.TimeIntervalPart = SelectedTimeIntervalPart.TimeIntervalPart;
				SelectedTimeIntervalPart.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		private bool CanEdit()
		{
			return !IsDefault && SelectedTimeIntervalPart != null;
		}

		public bool IsEnabled
		{
			get { return !IsDefault; }
		}
		public bool IsDefault
		{
			get { return Index == -1; }
		}

	}
}