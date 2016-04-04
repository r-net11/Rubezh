using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class HolidaysViewModel : OrganisationBaseViewModel<Holiday, HolidayFilter, HolidayViewModel, HolidayDetailsViewModel>, ISelectable<Guid>, ITimeTrackItemsViewModel
	{
		public HolidaysViewModel():base()
		{
			SelectedDate = DateTime.Now;
			_changeIsDeletedSubscriber = new ChangeIsDeletedSubscriber(this);
		}

		public LogicalDeletationType LogicalDeletationType { get; set; }
		ChangeIsDeletedSubscriber _changeIsDeletedSubscriber;

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

		public int SelectedYear
		{
			get { return _selectedDate.Year; }
		}

		private DateTime _selectedDate;
		public DateTime SelectedDate
		{
			get { return _selectedDate; }
			set
			{
				if (_selectedDate == value)
					return;
				_selectedDate = value;
				OnPropertyChanged(() => SelectedDate);
				OnPropertyChanged(() => SelectedYear);

				var filter = new HolidayFilter { UserUID = FiresecManager.CurrentUser.UID, Year = value.Year, LogicalDeletationType = LogicalDeletationType };
				Initialize(filter);
			}
		}

		public void Initialize()
		{
			var filter = new HolidayFilter { UserUID = FiresecManager.CurrentUser.UID, Year = SelectedYear, LogicalDeletationType = LogicalDeletationType };
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
			get { return "праздничный день"; }
		}


		protected override FiresecAPI.Models.PermissionType Permission
		{
			get { return FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Holidays_Edit; }
		}

		protected override bool IsAddViewModel(Holiday model)
		{
			return model.Date.Year == SelectedYear && base.IsAddViewModel(model);
		}
	}
}