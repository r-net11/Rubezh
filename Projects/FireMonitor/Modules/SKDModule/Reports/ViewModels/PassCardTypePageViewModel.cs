using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;

namespace SKDModule.Reports.ViewModels
{
	public class PassCardTypePageViewModel : FilterContainerViewModel
	{
		public PassCardTypePageViewModel()
		{
			Title = "Типы пропусков";
		}

		bool _passCardActive;
		public bool PassCardActive
		{
			get { return _passCardActive; }
			set
			{
				_passCardActive = value;
				OnPropertyChanged(() => PassCardActive);
				if (PassCardActive)
				{
					PassCardPermanent = true;
					PassCardTemprorary = true;
					PassCardGuest = true;
					PassCardForcing = true;
					PassCardLocked = true;
				}
			}
		}
		bool _passCardInactive;
		public bool PassCardInactive
		{
			get { return _passCardInactive; }
			set
			{
				_passCardInactive = value;
				OnPropertyChanged(() => PassCardInactive);
			}
		}
		bool _passCardPermanent;
		public bool PassCardPermanent
		{
			get { return _passCardPermanent; }
			set
			{
				_passCardPermanent = value;
				OnPropertyChanged(() => PassCardPermanent);
			}
		}
		bool _passCardTemprorary;
		public bool PassCardTemprorary
		{
			get { return _passCardTemprorary; }
			set
			{
				_passCardTemprorary = value;
				OnPropertyChanged(() => PassCardTemprorary);
			}
		}
		bool _passCardGuest;
		public bool PassCardGuest
		{
			get { return _passCardGuest; }
			set
			{
				_passCardGuest = value;
				OnPropertyChanged(() => PassCardGuest);
			}
		}
		bool _passCardForcing;
		public bool PassCardForcing
		{
			get { return _passCardForcing; }
			set
			{
				_passCardForcing = value;
				OnPropertyChanged(() => PassCardForcing);
			}
		}
		bool _passCardLocked;
		public bool PassCardLocked
		{
			get { return _passCardLocked; }
			set
			{
				_passCardLocked = value;
				OnPropertyChanged(() => PassCardLocked);
			}
		}

		bool _allowInactive;
		public bool AllowInactive
		{
			get { return _allowInactive; }
			set
			{
				_allowInactive = value;
				OnPropertyChanged(() => AllowInactive);
			}
		}

		public override void LoadFilter(SKDReportFilter filter)
		{
			var passCardTypeFilter = filter as IReportFilterPassCardType;
			if (passCardTypeFilter == null)
				return;
			PassCardActive = passCardTypeFilter.PassCardActive;
			PassCardPermanent = passCardTypeFilter.PassCardPermanent;
			PassCardTemprorary = passCardTypeFilter.PassCardTemprorary;
			PassCardGuest = passCardTypeFilter.PassCardGuest;
			PassCardForcing = passCardTypeFilter.PassCardForcing;
			PassCardLocked = passCardTypeFilter.PassCardLocked;
			var fullPassCardTypeFilter = passCardTypeFilter as IReportFilterPassCardTypeFull;
			AllowInactive = fullPassCardTypeFilter != null;
			if (AllowInactive)
				PassCardInactive = fullPassCardTypeFilter.PassCardInactive;
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var passCardTypeFilter = filter as IReportFilterPassCardType;
			if (passCardTypeFilter == null)
				return;
			passCardTypeFilter.PassCardActive = PassCardActive;
			passCardTypeFilter.PassCardPermanent = PassCardPermanent;
			passCardTypeFilter.PassCardTemprorary = PassCardTemprorary;
			passCardTypeFilter.PassCardGuest = PassCardGuest;
			passCardTypeFilter.PassCardForcing = PassCardForcing;
			passCardTypeFilter.PassCardLocked = PassCardLocked;
			var fullPassCardTypeFilter = passCardTypeFilter as IReportFilterPassCardTypeFull;
			if (fullPassCardTypeFilter != null)
				fullPassCardTypeFilter.PassCardInactive = PassCardInactive;
		}
	}
}