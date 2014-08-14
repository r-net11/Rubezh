using System;
using System.Collections.Generic;
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
	public class DayIntervalsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		private DayInterval _clipboard;
		private bool _isInitialized;

		public DayIntervalsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);

			_isInitialized = false;
		}

		public void Initialize()
		{
			var organisations = OrganisationHelper.GetByCurrentUser();
			if (organisations == null)
				return;
			var filter = new DayIntervalFilter()
			{
				UserUID = FiresecManager.CurrentUser.UID,
				OrganisationUIDs = organisations.Select(item => item.UID).ToList(),
			};
			var dayIntervals = DayIntervalHelper.Get(filter);

			AllDayIntervals = new List<DayIntervalViewModel>();
			Organisations = new List<DayIntervalViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new DayIntervalViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllDayIntervals.Add(organisationViewModel);
				foreach (var dayInterval in dayIntervals)
				{
					if (dayInterval.OrganisationUID == organisation.UID)
					{
						var dayIntervalViewModel = new DayIntervalViewModel(organisation, dayInterval);
						organisationViewModel.AddChild(dayIntervalViewModel);
						AllDayIntervals.Add(dayIntervalViewModel);
					}
				}
			}
			OnPropertyChanged(() => Organisations);
			SelectedDayInterval = Organisations.FirstOrDefault();
		}
		public override void OnShow()
		{
			base.OnShow();
			if (!_isInitialized)
			{
				Initialize();
				_isInitialized = true;
			}
		}

		private List<DayIntervalViewModel> AllDayIntervals { get; set; }
		public List<DayIntervalViewModel> Organisations { get; private set; }

		public void Select(Guid dayIntervalUID)
		{
			if (dayIntervalUID != Guid.Empty)
			{
				var dayIntervalViewModel = AllDayIntervals.FirstOrDefault(x => x.DayInterval != null && x.DayInterval.UID == dayIntervalUID);
				if (dayIntervalViewModel != null)
					dayIntervalViewModel.ExpandToThis();
				SelectedDayInterval = dayIntervalViewModel;
			}
		}

		private DayIntervalViewModel _selectedDayInterval;
		public DayIntervalViewModel SelectedDayInterval
		{
			get { return _selectedDayInterval; }
			set
			{
				_selectedDayInterval = value;
				if (value != null)
				{
					value.ExpandToThis();
					value.Initialize();
				}
				OnPropertyChanged(() => SelectedDayInterval);
			}
		}

		public DayIntervalViewModel ParentOrganisation
		{
			get
			{
				DayIntervalViewModel OrganisationViewModel = SelectedDayInterval;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedDayInterval.Parent;

				if (OrganisationViewModel.Organisation != null)
					return OrganisationViewModel;

				return null;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var DayIntervalDetailsViewModel = new DayIntervalDetailsViewModel(SelectedDayInterval.Organisation);
			if (DialogService.ShowModalWindow(DayIntervalDetailsViewModel))
			{
				var dayIntervalViewModel = new DayIntervalViewModel(SelectedDayInterval.Organisation, DayIntervalDetailsViewModel.DayInterval);

				DayIntervalViewModel OrganisationViewModel = SelectedDayInterval;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedDayInterval.Parent;

				if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
					return;

				OrganisationViewModel.AddChild(dayIntervalViewModel);
				SelectedDayInterval = dayIntervalViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedDayInterval != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			DayIntervalViewModel OrganisationViewModel = SelectedDayInterval;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedDayInterval.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedDayInterval);
			var dayInterval = SelectedDayInterval.DayInterval;
			bool removeResult = DayIntervalHelper.MarkDeleted(dayInterval);
			if (!removeResult)
				return;
			OrganisationViewModel.RemoveChild(SelectedDayInterval);
			index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
			if (index > -1)
				SelectedDayInterval = OrganisationViewModel.Children.ToList()[index];
			else
				SelectedDayInterval = OrganisationViewModel;
		}
		bool CanRemove()
		{
			return SelectedDayInterval != null && !SelectedDayInterval.IsOrganisation;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var dayIntervalDetailsViewModel = new DayIntervalDetailsViewModel(SelectedDayInterval.Organisation, SelectedDayInterval.DayInterval);
			if (DialogService.ShowModalWindow(dayIntervalDetailsViewModel))
				SelectedDayInterval.Update();
		}
		bool CanEdit()
		{
			return SelectedDayInterval != null && SelectedDayInterval.Parent != null && !SelectedDayInterval.IsOrganisation;
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_clipboard = CopyInterval(SelectedDayInterval.DayInterval, false);
		}
		private bool CanCopy()
		{
			return SelectedDayInterval != null && !SelectedDayInterval.IsOrganisation;
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newInterval = CopyInterval(_clipboard);
			if (DayIntervalHelper.Save(newInterval))
			{
				var timeInrervalViewModel = new DayIntervalViewModel(SelectedDayInterval.Organisation, newInterval);
				if (ParentOrganisation != null)
				{
					ParentOrganisation.AddChild(timeInrervalViewModel);
					AllDayIntervals.Add(timeInrervalViewModel);
				}
				SelectedDayInterval = timeInrervalViewModel;
			}
		}
		private bool CanPaste()
		{
			return SelectedDayInterval != null && _clipboard != null;
		}

		private DayInterval CopyInterval(DayInterval source, bool newName = true)
		{
			var copy = new DayInterval();
			copy.Name = newName ? CopyHelper.CopyName(source.Name, ParentOrganisation.Children.Select(item => item.Name)) : source.Name;
			copy.Description = source.Description;
			copy.SlideTime = source.SlideTime;
			copy.OrganisationUID = ParentOrganisation.Organisation.UID;
			foreach (var dayIntervalPart in source.DayIntervalParts)
				if (!dayIntervalPart.IsDeleted)
					copy.DayIntervalParts.Add(new DayIntervalPart()
					{
						BeginTime = dayIntervalPart.BeginTime,
						EndTime = dayIntervalPart.EndTime,
						TransitionType = dayIntervalPart.TransitionType,
						DayIntervalUID = copy.UID,
					});
			return copy;
		}
	}
}