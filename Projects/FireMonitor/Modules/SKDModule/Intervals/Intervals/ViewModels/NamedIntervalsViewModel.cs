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
	public class NamedIntervalsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		private NamedInterval _clipboard;
		private bool _isInitialized;

		public NamedIntervalsViewModel()
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
			var filter = new NamedIntervalFilter()
			{
				UserUID = FiresecManager.CurrentUser.UID,
				OrganisationUIDs = organisations.Select(item => item.UID).ToList(),
			};
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
			OnPropertyChanged(() => Organisations);
			SelectedNamedInterval = Organisations.FirstOrDefault();
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

		private List<NamedIntervalViewModel> AllNamedIntervals { get; set; }
		public List<NamedIntervalViewModel> Organisations { get; private set; }

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

		private NamedIntervalViewModel _selectedNamedInterval;
		public NamedIntervalViewModel SelectedNamedInterval
		{
			get { return _selectedNamedInterval; }
			set
			{
				_selectedNamedInterval = value;
				if (value != null)
				{
					value.ExpandToThis();
					value.Initialize();
				}
				OnPropertyChanged(() => SelectedNamedInterval);
			}
		}

		public NamedIntervalViewModel ParentOrganisation
		{
			get
			{
				NamedIntervalViewModel OrganisationViewModel = SelectedNamedInterval;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedNamedInterval.Parent;

				if (OrganisationViewModel.Organisation != null)
					return OrganisationViewModel;

				return null;
			}
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
				SelectedNamedInterval.Update();
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
			return SelectedNamedInterval != null && !SelectedNamedInterval.IsOrganisation;
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newInterval = CopyInterval(_clipboard);
			if (NamedIntervalHelper.Save(newInterval))
			{
				var timeInrervalViewModel = new NamedIntervalViewModel(SelectedNamedInterval.Organisation, newInterval);
				if (ParentOrganisation != null)
				{
					ParentOrganisation.AddChild(timeInrervalViewModel);
					AllNamedIntervals.Add(timeInrervalViewModel);
				}
				SelectedNamedInterval = timeInrervalViewModel;
			}
		}
		private bool CanPaste()
		{
			return SelectedNamedInterval != null && _clipboard != null;
		}

		private NamedInterval CopyInterval(NamedInterval source, bool newName = true)
		{
			var copy = new NamedInterval();
			copy.Name = newName ? CopyHelper.CopyName(source.Name, ParentOrganisation.Children.Select(item => item.Name)) : source.Name;
			copy.Description = source.Description;
			copy.SlideTime = source.SlideTime;
			copy.OrganisationUID = ParentOrganisation.Organisation.UID;
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