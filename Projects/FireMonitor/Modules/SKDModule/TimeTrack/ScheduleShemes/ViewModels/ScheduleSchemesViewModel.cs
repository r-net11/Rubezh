using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Common;

namespace SKDModule.ViewModels
{
	public abstract class ScheduleSchemesViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public abstract ScheduleSchemeType Type { get; }
		ScheduleScheme _clipboard;
		bool _isInitialized;
		Dictionary<Guid, ObservableCollection<DayInterval>> _dayIntervals;

		public ScheduleSchemesViewModel()
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
			var filter = new ScheduleSchemeFilter()
			{
				UserUID = FiresecManager.CurrentUser.UID,
				OrganisationUIDs = organisations.Select(item => item.UID).ToList(),
				Type = Type,
			};
			var scheduleSchemes = ScheduleSchemaHelper.Get(filter);

			AllScheduleSchemes = new List<ScheduleSchemeViewModel>();
			Organisations = new List<ScheduleSchemeViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new ScheduleSchemeViewModel(this, organisation);
				Organisations.Add(organisationViewModel);
				AllScheduleSchemes.Add(organisationViewModel);
				foreach (var scheduleScheme in scheduleSchemes)
				{
					if (scheduleScheme.OrganisationUID == organisation.UID)
					{
						var scheduleSchemeViewModel = new ScheduleSchemeViewModel(this, organisation, scheduleScheme);
						organisationViewModel.AddChild(scheduleSchemeViewModel);
						AllScheduleSchemes.Add(scheduleSchemeViewModel);
					}
				}
			}
			OnPropertyChanged(() => Organisations);
			SelectedScheduleScheme = Organisations.FirstOrDefault();
		}
		public override void OnShow()
		{
			base.OnShow();
			if (!_isInitialized)
			{
				Initialize();
				_isInitialized = true;
			}
			ReloadDayIntervals();
		}

		public void ReloadDayIntervals()
		{
			if (Organisations != null)
			{
				var dayIntervals = DayIntervalHelper.Get(new DayIntervalFilter()
				{
					UserUID = FiresecManager.CurrentUser.UID,
					OrganisationUIDs = Organisations.Select(item => item.Organisation.UID).ToList(),
				});
				_dayIntervals = new Dictionary<Guid, ObservableCollection<DayInterval>>();
				Organisations.ForEach(item => _dayIntervals.Add(item.Organisation.UID, new ObservableCollection<DayInterval>()));
				dayIntervals.ForEach(item => _dayIntervals[item.OrganisationUID].Add(item));
				_dayIntervals.Values.ForEach(item => item.Insert(0, new DayInterval()
				{
					UID = Guid.Empty,
					Name = "Никогда",
				}));
			}
		}
		public ObservableCollection<DayInterval> GetDayIntervals(Guid organisationUID)
		{
			return _dayIntervals.ContainsKey(organisationUID) ? _dayIntervals[organisationUID] : new ObservableCollection<DayInterval>();
		}

		public List<ScheduleSchemeViewModel> Organisations { get; private set; }
		List<ScheduleSchemeViewModel> AllScheduleSchemes { get; set; }

		public void Select(Guid scheduleSchemelUID)
		{
			if (scheduleSchemelUID != Guid.Empty)
			{
				var scheduleSchemeViewModel = AllScheduleSchemes.FirstOrDefault(x => x.ScheduleScheme != null && x.ScheduleScheme.UID == scheduleSchemelUID);
				if (scheduleSchemeViewModel != null)
					scheduleSchemeViewModel.ExpandToThis();
				SelectedScheduleScheme = scheduleSchemeViewModel;
			}
		}

		ScheduleSchemeViewModel _selectedScheduleScheme;
		public ScheduleSchemeViewModel SelectedScheduleScheme
		{
			get { return _selectedScheduleScheme; }
			set
			{
				_selectedScheduleScheme = value;
				if (value != null)
				{
					value.ExpandToThis();
					value.Initialize();
				}
				OnPropertyChanged(() => SelectedScheduleScheme);
			}
		}

		public ScheduleSchemeViewModel ParentOrganisation
		{
			get
			{
				ScheduleSchemeViewModel OrganisationViewModel = SelectedScheduleScheme;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedScheduleScheme.Parent;

				if (OrganisationViewModel.Organisation != null)
					return OrganisationViewModel;

				return null;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var ScheduleSchemeDetailsViewModel = new ScheduleSchemeDetailsViewModel(SelectedScheduleScheme.Organisation, Type);
			if (DialogService.ShowModalWindow(ScheduleSchemeDetailsViewModel))
			{
				var scheduleSchemeViewModel = new ScheduleSchemeViewModel(this, SelectedScheduleScheme.Organisation, ScheduleSchemeDetailsViewModel.ScheduleScheme);

				ScheduleSchemeViewModel OrganisationViewModel = SelectedScheduleScheme;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedScheduleScheme.Parent;

				if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
					return;

				OrganisationViewModel.AddChild(scheduleSchemeViewModel);
				SelectedScheduleScheme = scheduleSchemeViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedScheduleScheme != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion2("Вы уверены, что хотите удалить схему работы?"))
			{
				ScheduleSchemeViewModel OrganisationViewModel = SelectedScheduleScheme;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedScheduleScheme.Parent;

				if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
					return;

				var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedScheduleScheme);
				var scheduleScheme = SelectedScheduleScheme.ScheduleScheme;
				bool removeResult = ScheduleSchemaHelper.MarkDeleted(scheduleScheme);
				if (!removeResult)
					return;
				OrganisationViewModel.RemoveChild(SelectedScheduleScheme);
				index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
				if (index > -1)
					SelectedScheduleScheme = OrganisationViewModel.Children.ToList()[index];
				else
					SelectedScheduleScheme = OrganisationViewModel;
			}
		}
		bool CanRemove()
		{
			return SelectedScheduleScheme != null && !SelectedScheduleScheme.IsOrganisation;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var scheduleSchemeDetailsViewModel = new ScheduleSchemeDetailsViewModel(SelectedScheduleScheme.Organisation, Type, SelectedScheduleScheme.ScheduleScheme);
			if (DialogService.ShowModalWindow(scheduleSchemeDetailsViewModel))
				SelectedScheduleScheme.Update();
		}
		private bool CanEdit()
		{
			return SelectedScheduleScheme != null && SelectedScheduleScheme.Parent != null && !SelectedScheduleScheme.IsOrganisation;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			_clipboard = CopyScheduleScheme(SelectedScheduleScheme.ScheduleScheme, false);
		}
		bool CanCopy()
		{
			return SelectedScheduleScheme != null && !SelectedScheduleScheme.IsOrganisation;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var newInterval = CopyScheduleScheme(_clipboard);
			if (ScheduleSchemaHelper.Save(newInterval))
			{
				var scheduleSchemeViewModel = new ScheduleSchemeViewModel(this, SelectedScheduleScheme.Organisation, newInterval);
				if (ParentOrganisation != null)
				{
					ParentOrganisation.AddChild(scheduleSchemeViewModel);
					AllScheduleSchemes.Add(scheduleSchemeViewModel);
				}
				SelectedScheduleScheme = scheduleSchemeViewModel;
			}
		}
		bool CanPaste()
		{
			return SelectedScheduleScheme != null && _clipboard != null && _clipboard.Type == Type;
		}

		ScheduleScheme CopyScheduleScheme(ScheduleScheme source, bool newName = true)
		{
			var copy = new ScheduleScheme();
			copy.Type = Type;
			copy.Name = newName ? CopyHelper.CopyName(source.Name, ParentOrganisation.Children.Select(item => item.Name)) : source.Name;
			copy.Description = source.Description;
			copy.OrganisationUID = ParentOrganisation.Organisation.UID;
			foreach (var day in source.DayIntervals)
				if (!day.IsDeleted)
					copy.DayIntervals.Add(new ScheduleDayInterval()
					{
						DayIntervalUID = day.DayIntervalUID,
						Number = day.Number,
						ScheduleSchemeUID = copy.UID,
					});
			return copy;
		}
	}
}