using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class HolidaysViewModel : OrganisationBaseViewModel<Holiday, HolidayFilter, HolidayViewModel, HolidayDetailsViewModel>, ISelectable<Guid>
	{
		public HolidaysViewModel():base()
		{
			InitializeYears();
		}

		public LogicalDeletationType LogicalDeletationType { get; set; }
		
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
			copy.Date = new DateTime(SelectedYear, source.Date.Month, source.Date.Day);
			if(source.TransferDate != null)
				copy.TransferDate =  new DateTime(SelectedYear, source.TransferDate.Value.Month, source.TransferDate.Value.Day);
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

				var filter = new HolidayFilter() { UserUID = ClientManager.CurrentUser.UID, Year = value, LogicalDeletationType = LogicalDeletationType };
				Initialize(filter);
			}
		}

		public void Initialize()
		{
			var filter = new HolidayFilter() { UserUID = ClientManager.CurrentUser.UID, Year = SelectedYear, LogicalDeletationType = LogicalDeletationType };
			Initialize(filter);
		}

		protected override bool Add(Holiday item)
		{
			return HolidayHelper.Save(item, true);
		}

		protected override System.Collections.Generic.IEnumerable<Holiday> GetModels(HolidayFilter filter)
		{
			return HolidayHelper.Get(filter);
		}

		protected override System.Collections.Generic.IEnumerable<Holiday> GetModelsByOrganisation(Guid organisauinUID)
		{
			return HolidayHelper.GetByOrganisation(organisauinUID);
		}

		protected override bool MarkDeleted(Holiday model)
		{
			return HolidayHelper.MarkDeleted(model);
		}

		protected override bool Restore(Holiday model)
		{
			return HolidayHelper.Restore(model);
		}

		protected override string ItemRemovingName
		{
			get { return "сокращённый день"; }
		}


		protected override RubezhAPI.Models.PermissionType Permission
		{
			get { return RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Holidays_Edit; }
		}

		protected override bool IsAddViewModel(Holiday model)
		{
			return model.Date.Year == SelectedYear && base.IsAddViewModel(model);
		}
	}
}