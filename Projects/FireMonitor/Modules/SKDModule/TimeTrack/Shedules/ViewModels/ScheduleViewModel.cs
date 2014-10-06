using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ScheduleViewModel : CartothequeTabItemElementBase<ScheduleViewModel, Schedule>, IEditingViewModel
	{
		private bool _isInitialized;
		
		public ScheduleViewModel() { }
		
		public override void InitializeOrganisation(Organisation organisation, ViewPartViewModel parentViewModel)
		{
			base.InitializeOrganisation(organisation, parentViewModel);
			_isInitialized = false;
		}
		
		public override void InitializeModel(Organisation organisation, Schedule model, ViewPartViewModel parentViewModel)
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			base.InitializeModel(organisation, model, parentViewModel);
			_isInitialized = false;
			Update();
		}

		public void Initialize()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				if (!IsOrganisation)
				{
					ScheduleZones = new SortableObservableCollection<ScheduleZoneViewModel>();
					foreach (var employeeScheduleZone in Model.Zones)
					{
						var scheduleZoneViewModel = new ScheduleZoneViewModel(employeeScheduleZone);
						ScheduleZones.Add(scheduleZoneViewModel);
					}
					SelectedScheduleZone = ScheduleZones.FirstOrDefault();
				}
			}
		}
		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => Scheme);
		}

		public string Scheme
		{
			get
			{
				if (!IsOrganisation)
				{
					var schemes = ScheduleSchemaHelper.Get(new ScheduleSchemeFilter()
					{
						OrganisationUIDs = new List<Guid>() { Organisation.UID },
						Type = ScheduleSchemeType.Month | ScheduleSchemeType.SlideDay | ScheduleSchemeType.Week,
						WithDays = false,
					});
					var scheme = schemes.FirstOrDefault(item => item.UID == Model.ScheduleSchemeUID);
					if (scheme != null)
					{
						return scheme.Name + " (" + scheme.Type.ToDescription() + ")";
					}
				}
				return null;
			}
		}

		public SortableObservableCollection<ScheduleZoneViewModel> ScheduleZones { get; private set; }

		private ScheduleZoneViewModel _selectedScheduleZone;
		public ScheduleZoneViewModel SelectedScheduleZone
		{
			get { return _selectedScheduleZone; }
			set
			{
				_selectedScheduleZone = value;
				OnPropertyChanged(() => SelectedScheduleZone);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var scheduleZoneDetailsViewModel = new ScheduleZoneDetailsViewModel(Model);
			if (DialogService.ShowModalWindow(scheduleZoneDetailsViewModel) && ScheduleZoneHelper.Save(scheduleZoneDetailsViewModel.ScheduleZone))
			{
				var scheduleZone = scheduleZoneDetailsViewModel.ScheduleZone;
				Model.Zones.Add(scheduleZone);
				var scheduleZoneViewModel = new ScheduleZoneViewModel(scheduleZone);
				ScheduleZones.Add(scheduleZoneViewModel);
				Sort();
				SelectedScheduleZone = scheduleZoneViewModel;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		private void OnDelete()
		{
			if (ScheduleZoneHelper.MarkDeleted(SelectedScheduleZone.Model))
			{
				Model.Zones.Remove(SelectedScheduleZone.Model);
				ScheduleZones.Remove(SelectedScheduleZone);
			}
		}
		private bool CanDelete()
		{
			return SelectedScheduleZone != null && ScheduleZones.Count > 1;
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var scheduleZoneDetailsViewModel = new ScheduleZoneDetailsViewModel(Model, SelectedScheduleZone.Model);
			if (DialogService.ShowModalWindow(scheduleZoneDetailsViewModel))
			{
				ScheduleZoneHelper.Save(SelectedScheduleZone.Model);
				var selectedScheduleZone = SelectedScheduleZone;
				SelectedScheduleZone.Update();
				Sort();
				SelectedScheduleZone = selectedScheduleZone;
			}
		}
		private bool CanEdit()
		{
			return SelectedScheduleZone != null;
		}
	}
}