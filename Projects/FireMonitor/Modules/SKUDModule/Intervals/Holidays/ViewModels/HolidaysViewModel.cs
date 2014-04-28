using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Common;

namespace SKDModule.ViewModels
{
	public class HolidaysViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		HolidayFilter Filter;
		Holiday _clipboard;

		public HolidaysViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			EditFilterCommand = new RelayCommand(OnEditFilter);

			InitializeYears();
			//Filter = new HolidayFilter() { OrganisationUIDs = FiresecManager.CurrentUser.OrganisationUIDs };
			//Initialize(Filter);
		}

		public void Initialize(HolidayFilter filter)
		{
			var organisations = OrganisationHelper.Get(new FiresecAPI.OrganisationFilter() { UIDs = FiresecManager.CurrentUser.OrganisationUIDs });
			var holidays = HolidayHelper.Get(filter);

			AllHolidays = new List<HolidayViewModel>();
			Organisations = new List<HolidayViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new HolidayViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllHolidays.Add(organisationViewModel);
				foreach (var holiday in holidays)
				{
					if (holiday.OrganisationUID == organisation.UID)
					{
						var holidayViewModel = new HolidayViewModel(organisation, holiday);
						organisationViewModel.AddChild(holidayViewModel);
						AllHolidays.Add(holidayViewModel);
					}
				}
			}
			OnPropertyChanged("Organisations");
			SelectedHoliday = Organisations.FirstOrDefault();
		}

		public List<HolidayViewModel> Organisations { get; private set; }
		List<HolidayViewModel> AllHolidays { get; set; }

		public void Select(Guid holidayUID)
		{
			if (holidayUID != Guid.Empty)
			{
				var holidayViewModel = AllHolidays.FirstOrDefault(x => x.Holiday != null && x.Holiday.UID == holidayUID);
				if (holidayViewModel != null)
					holidayViewModel.ExpandToThis();
				SelectedHoliday = holidayViewModel;
			}
		}

		HolidayViewModel _selectedHoliday;
		public HolidayViewModel SelectedHoliday
		{
			get { return _selectedHoliday; }
			set
			{
				_selectedHoliday = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("SelectedHoliday");
			}
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			//var filterViewModel = new HolidayFilterViewModel(Filter);
			//if (DialogService.ShowModalWindow(filterViewModel))
			//{
			//    Filter = filterViewModel.Filter;
			//    Initialize(Filter);
			//}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var holidayDetailsViewModel = new HolidayDetailsViewModel(SelectedHoliday.Organisation, SelectedYear.Year);
			if (DialogService.ShowModalWindow(holidayDetailsViewModel))
			{
				var holidayViewModel = new HolidayViewModel(SelectedHoliday.Organisation, holidayDetailsViewModel.Holiday);

				HolidayViewModel OrganisationViewModel = SelectedHoliday;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedHoliday.Parent;

				if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
					return;

				OrganisationViewModel.AddChild(holidayViewModel);
				SelectedHoliday = holidayViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedHoliday != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			HolidayViewModel OrganisationViewModel = SelectedHoliday;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedHoliday.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedHoliday);
			var holiday = SelectedHoliday.Holiday;
			bool removeResult = HolidayHelper.MarkDeleted(holiday);
			if (!removeResult)
				return;
			OrganisationViewModel.RemoveChild(SelectedHoliday);
			index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
			if (index > -1)
				SelectedHoliday = OrganisationViewModel.Children.ToList()[index];
			else
				SelectedHoliday = OrganisationViewModel;
		}
		bool CanRemove()
		{
			return SelectedHoliday != null && !SelectedHoliday.IsOrganisation;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var holidayDetailsViewModel = new HolidayDetailsViewModel(SelectedHoliday.Organisation, SelectedYear.Year, SelectedHoliday.Holiday);
			if (DialogService.ShowModalWindow(holidayDetailsViewModel))
			{
				SelectedHoliday.Update(holidayDetailsViewModel.Holiday);
			}
		}
		bool CanEdit()
		{
			return SelectedHoliday != null && SelectedHoliday.Parent != null && !SelectedHoliday.IsOrganisation;
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_clipboard = CopyHoliday(SelectedHoliday.Holiday, false);
		}
		private bool CanCopy()
		{
			return SelectedHoliday != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newInterval = CopyHoliday(_clipboard);
			if (HolidayHelper.Save(newInterval))
			{
				var timeInrervalViewModel = new HolidayViewModel(SelectedHoliday.Organisation, newInterval);
				AllHolidays.Add(timeInrervalViewModel);
				SelectedHoliday = timeInrervalViewModel;
			}
		}
		private bool CanPaste()
		{
			return _clipboard != null;
		}

		private Holiday CopyHoliday(Holiday source, bool newName = true)
		{
			var copy = new Holiday();
			copy.Name = newName ? CopyHelper.CopyName(source.Name, AllHolidays.Select(item => item.Holiday.Name)) : source.Name;
			copy.OrganisationUID = source.OrganisationUID;
			return copy;
		}


		void InitializeYears()
		{
			AvailableYears = new ObservableCollection<HolidayYearViewModel>();
			for (int i = 2010; i <= 2020; i++)
				AvailableYears.Add(new HolidayYearViewModel(i));
			SelectedYear = AvailableYears.FirstOrDefault(x => x.Year == DateTime.Now.Year);
		}

		private ObservableCollection<HolidayYearViewModel> _availableYears;
		public ObservableCollection<HolidayYearViewModel> AvailableYears
		{
			get { return _availableYears; }
			set
			{
				_availableYears = value;
				OnPropertyChanged(() => AvailableYears);
			}
		}

		private HolidayYearViewModel _selectedYear;
		public HolidayYearViewModel SelectedYear
		{
			get { return _selectedYear; }
			set
			{
				_selectedYear = value;
				OnPropertyChanged(() => SelectedYear);

				Filter = new HolidayFilter() { OrganisationUIDs = FiresecManager.CurrentUser.OrganisationUIDs, Year = value.Year };
				Initialize(Filter);
			}
		}
	}
}