using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using OrganisationFilter = FiresecAPI.OrganisationFilter;
using System;
using SKDModule.Intervals.Common.ViewModels;
using Infrastructure.Common.Windows;
using SKDModule.Intervals.Common;

namespace SKDModule.ViewModels
{
	public class NamedIntervalsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		NamedIntervalFilter Filter;
		NamedInterval _clipboard;

		public NamedIntervalsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			EditFilterCommand = new RelayCommand(OnEditFilter);
			Filter = new NamedIntervalFilter() { OrganisationUIDs = FiresecManager.CurrentUser.OrganisationUIDs };
			Initialize(Filter);
		}

		public void Initialize(NamedIntervalFilter filter)
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { UIDs = FiresecManager.CurrentUser.OrganisationUIDs });
			var namedIntervals = NamedIntervalHelper.Get(filter);

			AllNamedIntervals = new List<NamedIntervalViewModel>();
			Organisations = new List<NamedIntervalViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new NamedIntervalViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllNamedIntervals.Add(organisationViewModel);
				foreach (var namedInterval in namedIntervals)
				{
					if (namedInterval.OrganisationUID == organisation.UID)
					{
						var namedIntervalViewModel = new NamedIntervalViewModel(organisation, namedInterval);
						organisationViewModel.AddChild(namedIntervalViewModel);
						AllNamedIntervals.Add(namedIntervalViewModel);
					}
				}
			}
			OnPropertyChanged("Organisations");
			SelectedNamedInterval = Organisations.FirstOrDefault();
		}

		public List<NamedIntervalViewModel> Organisations { get; private set; }
		List<NamedIntervalViewModel> AllNamedIntervals { get; set; }

		public void Select(Guid namedIntervalUID)
		{
			if (namedIntervalUID != Guid.Empty)
			{
				var namedIntervalViewModel = AllNamedIntervals.FirstOrDefault(x => x.NamedInterval != null && x.NamedInterval.UID == namedIntervalUID);
				if (namedIntervalViewModel != null)
					namedIntervalViewModel.ExpandToThis();
				SelectedNamedInterval = namedIntervalViewModel;
			}
		}

		NamedIntervalViewModel _selectedNamedInterval;
		public NamedIntervalViewModel SelectedNamedInterval
		{
			get { return _selectedNamedInterval; }
			set
			{
				_selectedNamedInterval = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("SelectedNamedInterval");
			}
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			//var filterViewModel = new NamedIntervalFilterViewModel(Filter);
			//if (DialogService.ShowModalWindow(filterViewModel))
			//{
			//    Filter = filterViewModel.Filter;
			//    Initialize(Filter);
			//}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var NamedIntervalDetailsViewModel = new NamedIntervalDetailsViewModel(SelectedNamedInterval.Organisation);
			if (DialogService.ShowModalWindow(NamedIntervalDetailsViewModel))
			{
				var namedIntervalViewModel = new NamedIntervalViewModel(SelectedNamedInterval.Organisation, NamedIntervalDetailsViewModel.NamedInterval);

				NamedIntervalViewModel OrganisationViewModel = SelectedNamedInterval;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedNamedInterval.Parent;

				if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
					return;

				OrganisationViewModel.AddChild(namedIntervalViewModel);
				SelectedNamedInterval = namedIntervalViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedNamedInterval != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			NamedIntervalViewModel OrganisationViewModel = SelectedNamedInterval;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedNamedInterval.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedNamedInterval);
			var namedInterval = SelectedNamedInterval.NamedInterval;
			bool removeResult = NamedIntervalHelper.MarkDeleted(namedInterval);
			if (!removeResult)
				return;
			OrganisationViewModel.RemoveChild(SelectedNamedInterval);
			index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
			if (index > -1)
				SelectedNamedInterval = OrganisationViewModel.Children.ToList()[index];
			else
				SelectedNamedInterval = OrganisationViewModel;
		}
		bool CanRemove()
		{
			return SelectedNamedInterval != null && !SelectedNamedInterval.IsOrganisation;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var namedIntervalDetailsViewModel = new NamedIntervalDetailsViewModel(SelectedNamedInterval.Organisation, SelectedNamedInterval.NamedInterval);
			if (DialogService.ShowModalWindow(namedIntervalDetailsViewModel))
			{
				SelectedNamedInterval.Update(namedIntervalDetailsViewModel.NamedInterval);
			}
		}
		bool CanEdit()
		{
			return SelectedNamedInterval != null && SelectedNamedInterval.Parent != null && !SelectedNamedInterval.IsOrganisation;
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_clipboard = CopyInterval(SelectedNamedInterval.NamedInterval, false);
		}
		private bool CanCopy()
		{
			return SelectedNamedInterval != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newInterval = CopyInterval(_clipboard);
			if (NamedIntervalHelper.Save(newInterval))
			{
				var timeInrervalViewModel = new NamedIntervalViewModel(SelectedNamedInterval.Organisation, newInterval);
				AllNamedIntervals.Add(timeInrervalViewModel);
				SelectedNamedInterval = timeInrervalViewModel;
			}
		}
		private bool CanPaste()
		{
			return _clipboard != null;
		}

		private NamedInterval CopyInterval(NamedInterval source, bool newName = true)
		{
			var copy = new NamedInterval();
			copy.Name = newName ? CopyHelper.CopyName(source.Name, AllNamedIntervals.Select(item => item.NamedInterval.Name)) : source.Name;
			copy.Description = source.Description;
			copy.SlideTime = source.SlideTime;
			copy.OrganisationUID = source.OrganisationUID;
			foreach (var timeInterval in source.TimeIntervals)
				if (!timeInterval.IsDeleted)
					copy.TimeIntervals.Add(new TimeInterval()
					{
						BeginTime = timeInterval.BeginTime,
						EndTime = timeInterval.EndTime,
						IntervalTransitionType = timeInterval.IntervalTransitionType,
						NamedIntervalUID = copy.UID,
					});
			return copy;
		}
	}
}