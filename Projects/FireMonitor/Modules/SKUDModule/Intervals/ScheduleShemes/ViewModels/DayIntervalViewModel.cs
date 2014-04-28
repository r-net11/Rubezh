using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayIntervalViewModel : BaseObjectViewModel<DayInterval>
	{
		ScheduleSchemeViewModel _scheduleScheme;
		bool _initialized;

		public DayIntervalViewModel(ScheduleSchemeViewModel scheduleScheme, DayInterval dayInterval)
			: base(dayInterval)
		{
			_initialized = false;
			_scheduleScheme = scheduleScheme;
			Update();
			_scheduleScheme.ScheduleSchemesViewModel.PropertyChanged += OrganisationViewModel_PropertyChanged;
			_initialized = true;
		}

		public string Name { get; private set; }
		public ObservableCollection<NamedInterval> NamedIntervals
		{
			get { return _scheduleScheme.ScheduleSchemesViewModel.NamedIntervals; }
		}

		public override void Update()
		{
			base.Update();
			Name = _scheduleScheme.ScheduleScheme.Type == ScheduleSchemeType.Week ? ((DayOfWeek)((Model.Number + 1) % 7)).ToString() : (Model.Number + 1).ToString();
			SelectedNamedInterval = NamedIntervals.SingleOrDefault(item => item.UID == Model.NamedIntervalUID) ?? NamedIntervals[0];
			OnPropertyChanged(() => Name);
		}

		NamedInterval _selectedNamedInterval;
		public NamedInterval SelectedNamedInterval
		{
			get { return _selectedNamedInterval; }
			set
			{
				if (SelectedNamedInterval != value)
				{
					_selectedNamedInterval = value ?? NamedIntervals[0];
					if (_initialized || Model.NamedIntervalUID != _selectedNamedInterval.UID)
					{
						Model.NamedIntervalUID = _selectedNamedInterval.UID;
						DayIntervalHelper.Save(Model);
					}
				}
				OnPropertyChanged(() => SelectedNamedInterval);
			}
		}

		void OrganisationViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "NamedIntervals")
				Update();
		}
	}
}