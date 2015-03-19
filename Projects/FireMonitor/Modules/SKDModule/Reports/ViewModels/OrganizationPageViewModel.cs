using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;
using Infrastructure.Events;
using SKDModule.ViewModels;

namespace SKDModule.Reports.ViewModels
{
	public class OrganizationPageViewModel : FilterContainerViewModel
	{
		private ReportOrganisationsFilterViewModel _organisationsFilter;

		public OrganizationPageViewModel(bool allowMultiple)
		{
			Title = "Организации";
			AllowMultiple = allowMultiple;
			_organisationsFilter = new ReportOrganisationsFilterViewModel();
			Organisations = new ObservableCollection<ReportFilterOrganisationViewModel>(_organisationsFilter.Organisations.Items);
			SelectAllCommand = new RelayCommand(() => Organisations.ForEach(item => item.IsChecked = true));
			SelectNoneCommand = new RelayCommand(() => Organisations.ForEach(item => item.IsChecked = false));
			ServiceFactory.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Unsubscribe(OnUseArchive);
			ServiceFactory.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Subscribe(OnUseArchive);
		}

		public bool AllowMultiple { get; set; }
		public ObservableCollection<ReportFilterOrganisationViewModel> Organisations { get; private set; }
		public RelayCommand SelectAllCommand { get; private set; }
		public RelayCommand SelectNoneCommand { get; private set; }

		public override void LoadFilter(SKDReportFilter filter)
		{
			var organisationFilter = filter as IReportFilterOrganisation;
			var uids = organisationFilter == null ? null : organisationFilter.Organisations;
			if (!AllowMultiple && uids == null && _organisationsFilter.Organisations.Items.Count > 0)
				uids = new List<Guid>();
			_organisationsFilter.Initialize(uids);
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var organisationFilter = filter as IReportFilterOrganisation;
			if (organisationFilter != null)
				organisationFilter.Organisations = _organisationsFilter.UIDs;
		}
		void OnUseArchive(bool isWithDeleted)
		{
			var uids = _organisationsFilter.UIDs;
			_organisationsFilter = new ReportOrganisationsFilterViewModel(isWithDeleted);
			_organisationsFilter.Initialize(uids);
			Organisations = new ObservableCollection<ReportFilterOrganisationViewModel>(_organisationsFilter.Organisations.Items);
		}
	}
}
