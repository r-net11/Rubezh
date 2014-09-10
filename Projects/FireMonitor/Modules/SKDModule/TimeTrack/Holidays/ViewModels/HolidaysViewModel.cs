using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class HolidaysViewModel : OrganisationBaseViewModel<Holiday, HolidayFilter, HolidayViewModel, HolidayDetailsViewModel>, ISelectable<Guid>
	{
		public HolidaysViewModel():base()
		{
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
            InitializeYears();
		}

		public void Select(Guid holidayUID)
		{
			if (holidayUID != Guid.Empty)
			{
                var holidayViewModel = Organisations.SelectMany(x => x.Children).FirstOrDefault(x => x.Model != null && x.Model.UID == holidayUID);
				if (holidayViewModel != null)
					holidayViewModel.ExpandToThis();
				SelectedItem = holidayViewModel;
			}
		}

        protected override Holiday CopyModel(Holiday source)
        {
            var copy = base.CopyModel(source);
            copy.Type = source.Type;
            copy.Date = source.Date;
            copy.TransferDate = source.TransferDate;
            copy.Reduction = source.Reduction;
            return copy;
        }
        
        void InitializeYears()
		{
			AvailableYears = new ObservableCollection<int>();
			for (int i = 2014; i <= 2020; i++)
				AvailableYears.Add(i);
			SelectedYear = AvailableYears.FirstOrDefault(x => x == DateTime.Now.Year);
		}

		ObservableCollection<int> _availableYears;
		public ObservableCollection<int> AvailableYears
		{
			get { return _availableYears; }
			set
			{
				_availableYears = value;
				OnPropertyChanged(() => AvailableYears);
			}
		}

		int _selectedYear;
		public int SelectedYear
		{
			get { return _selectedYear; }
			set
			{
				_selectedYear = value;
				OnPropertyChanged(() => SelectedYear);

				var filter = new HolidayFilter() { UserUID = FiresecManager.CurrentUser.UID, Year = value };
				Initialize(filter);
			}
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		void OnShowSettings()
		{
			var nightSettingsViewModel = new NightSettingsViewModel(ParentOrganisation.Organisation.UID);
			DialogService.ShowModalWindow(nightSettingsViewModel);
		}

        protected override bool Save(Holiday item)
        {
            return HolidayHelper.Save(item);
        }

        protected override System.Collections.Generic.IEnumerable<Holiday> GetModels(HolidayFilter filter)
        {
            return HolidayHelper.Get(filter);
        }

        protected override System.Collections.Generic.IEnumerable<Holiday> GetModelsByOrganisation(Guid organisauinUID)
        {
            return HolidayHelper.GetByOrganisation(organisauinUID);
        }

        protected override bool MarkDeleted(Guid uid)
        {
            return HolidayHelper.MarkDeleted(uid);
        }

        protected override string ItemRemovingName
        {
            get { return "праздничный день"; }
        }
    }
}