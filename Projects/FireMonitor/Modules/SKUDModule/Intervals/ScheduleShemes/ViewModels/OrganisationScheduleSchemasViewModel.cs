using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKDModule.Intervals.Common.ViewModels;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient.SKDHelpers;
using System.Collections.ObjectModel;
using SKDModule.Intervals.Common;

namespace SKDModule.Intervals.ScheduleShemes.ViewModels
{
	public class OrganisationScheduleSchemasViewModel : OrganisationViewModel<ScheduleSchemeViewModel, ScheduleScheme>
	{
		public ScheduleSchemeType Type { get; private set; }
		public ObservableCollection<NamedInterval> NamedIntervals { get; private set; }
		private ScheduleScheme _clipboard;

		public OrganisationScheduleSchemasViewModel(ScheduleSchemeType type, FiresecAPI.Organisation organisation)
			: base(organisation)
		{
			Type = type;
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
		}

		public override void Initialize(IEnumerable<ScheduleScheme> models)
		{
			ReloadNamedIntervals();
			base.Initialize(models);
		}
		protected override ScheduleSchemeViewModel CreateViewModel(ScheduleScheme model)
		{
			return new ScheduleSchemeViewModel(this, model);
		}

		public void ReloadNamedIntervals()
		{
			var namedIntervals = NamedIntervalHelper.Get(new NamedIntervalFilter()
			{
				OrganisationUIDs = new List<Guid>()
				{
					Organisation.UID,
				},
			});
			NamedIntervals = new ObservableCollection<NamedInterval>(namedIntervals);
			NamedIntervals.Insert(0, new NamedInterval()
			{
				UID = Guid.Empty,
				Name = "Никогда",
			});
			OnPropertyChanged(() => NamedIntervals);
		}

		private void OnAdd()
		{
			var detailsViewModel = new ScheduleSchemeDetailsViewModel(this);
			if (DialogService.ShowModalWindow(detailsViewModel) && ScheduleSchemaHelper.Save(detailsViewModel.ScheduleSchema))
			{
				var viewModel = new ScheduleSchemeViewModel(this, detailsViewModel.ScheduleSchema);
				ViewModels.Add(viewModel);
				SelectedViewModel = viewModel;
			}
		}
		private void OnDelete()
		{
			if (ScheduleSchemaHelper.MarkDeleted(SelectedViewModel.Model))
				ViewModels.Remove(SelectedViewModel);
		}
		private bool CanDelete()
		{
			return SelectedViewModel != null;
		}
		private void OnEdit()
		{
			var detailsViewModel = new ScheduleSchemeDetailsViewModel(this, SelectedViewModel.Model);
			if (DialogService.ShowModalWindow(detailsViewModel))
			{
				ScheduleSchemaHelper.Save(SelectedViewModel.Model);
				SelectedViewModel.Update();
			}
		}
		private bool CanEdit()
		{
			return SelectedViewModel != null;
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_clipboard = CopyInterval(SelectedViewModel.Model, false);
		}
		private bool CanCopy()
		{
			return SelectedViewModel != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var model = CopyInterval(_clipboard);
			if (ScheduleSchemaHelper.Save(model))
			{
				var viewModel = new ScheduleSchemeViewModel(this, model);
				ViewModels.Add(viewModel);
				SelectedViewModel = viewModel;
			}
		}
		private bool CanPaste()
		{
			return _clipboard != null;
		}

		private ScheduleScheme CopyInterval(ScheduleScheme source, bool newName = true)
		{
			var copy = new ScheduleScheme();
			copy.Name = newName ? CopyHelper.CopyName(source.Name, ViewModels.Select(item => item.Model.Name)) : source.Name;
			copy.Description = source.Description;
			copy.OrganisationUID = source.OrganisationUID;
			foreach (var day in source.DayIntervals)
				if (!day.IsDeleted)
					copy.DayIntervals.Add(new DayInterval()
					{
						NamedIntervalUID = day.NamedIntervalUID,
						Number = day.Number,
						ScheduleSchemeUID = copy.UID,
					});
			return copy;
		}
	}
}
