using System;
using System.Linq;
using Common;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayIntervalViewModel : TreeNodeViewModel<DayIntervalViewModel>, IEditingViewModel
	{
		bool _isInitialized;
		public FiresecAPI.SKD.Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public DayInterval DayInterval { get; private set; }

		public DayIntervalViewModel(FiresecAPI.SKD.Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			IsExpanded = true;
			_isInitialized = true;
		}
		public DayIntervalViewModel(FiresecAPI.SKD.Organisation organisation, DayInterval dayInterval)
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);

			Organisation = organisation;
			DayInterval = dayInterval;
			IsOrganisation = false;

			_isInitialized = false;
		}

		public void Initialize()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				if (!IsOrganisation)
				{
					DayIntervalParts = new SortableObservableCollection<DayIntervalPartViewModel>();
					foreach (var dayIntervalPart in DayInterval.DayIntervalParts)
					{
						var dayIntervalPartViewModel = new DayIntervalPartViewModel(dayIntervalPart);
						DayIntervalParts.Add(dayIntervalPartViewModel);
					}
					SelectedDayIntervalPart = DayIntervalParts.FirstOrDefault();
				}
			}
		}
		public void Update()
		{
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
		}

		public string Name
		{
			get { return IsOrganisation ? Organisation.Name : DayInterval.Name; }
		}
		public string Description
		{
			get { return IsOrganisation ? Organisation.Description : DayInterval.Description; }
		}

		public SortableObservableCollection<DayIntervalPartViewModel> DayIntervalParts { get; private set; }

		DayIntervalPartViewModel _selectedDayIntervalPart;
		public DayIntervalPartViewModel SelectedDayIntervalPart
		{
			get { return _selectedDayIntervalPart; }
			set
			{
				_selectedDayIntervalPart = value;
				OnPropertyChanged(() => SelectedDayIntervalPart);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var dayIntervalPartDetailsViewModel = new DayIntervalPartDetailsViewModel(DayInterval);
			if (DialogService.ShowModalWindow(dayIntervalPartDetailsViewModel) && DayIntervalPartHelper.Save(dayIntervalPartDetailsViewModel.DayIntervalPart))
			{
				var dayIntervalPart = dayIntervalPartDetailsViewModel.DayIntervalPart;
				DayInterval.DayIntervalParts.Add(dayIntervalPart);
				var dayIntervalPartViewModel = new DayIntervalPartViewModel(dayIntervalPart);
				DayIntervalParts.Add(dayIntervalPartViewModel);
				DayIntervalParts.Sort(item => item.BeginTime);
				SelectedDayIntervalPart = dayIntervalPartViewModel;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (DayIntervalPartHelper.MarkDeleted(SelectedDayIntervalPart.DayIntervalPart))
			{
				DayInterval.DayIntervalParts.Remove(SelectedDayIntervalPart.DayIntervalPart);
				DayIntervalParts.Remove(SelectedDayIntervalPart);
			}
		}
		bool CanDelete()
		{
			return SelectedDayIntervalPart != null && DayIntervalParts.Count > 1;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var dayIntervalPartDetailsViewModel = new DayIntervalPartDetailsViewModel(DayInterval, SelectedDayIntervalPart.DayIntervalPart);
			if (DialogService.ShowModalWindow(dayIntervalPartDetailsViewModel))
			{
				DayIntervalPartHelper.Save(SelectedDayIntervalPart.DayIntervalPart);
				SelectedDayIntervalPart.Update();
				var selectedDayIntervalPart = SelectedDayIntervalPart;
				DayIntervalParts.Sort(item => item.BeginTime);
				SelectedDayIntervalPart = selectedDayIntervalPart;
			}
		}
		bool CanEdit()
		{
			return SelectedDayIntervalPart != null;
		}
	}
}