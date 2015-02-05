using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using FiresecAPI.SKD.ReportFilters;
using SKDModule.ViewModels;
using Infrastructure.Common.CheckBoxList;
using System.Collections.ObjectModel;
using Infrastructure.Common;

namespace SKDModule.Reports.ViewModels
{
	public class OrganizationPageViewModel : FilterContainerViewModel
	{
		private OrganisationsFilterViewModel _organisationsFilter;

		public OrganizationPageViewModel(bool allowMultiple)
		{
			Title = "Организации";
			AllowMultiple = allowMultiple;
			_organisationsFilter = new OrganisationsFilterViewModel();
			Organisations = new ObservableCollection<FilterOrganisationViewModel>(_organisationsFilter.Organisations.Items);
			SelectAllCommand = new RelayCommand(() => Organisations.ForEach(item => item.IsChecked = true));
			SelectNoneCommand = new RelayCommand(() => Organisations.ForEach(item => item.IsChecked = false));
		}

		public bool AllowMultiple { get; set; }
		public ObservableCollection<FilterOrganisationViewModel> Organisations { get; private set; }
		public RelayCommand SelectAllCommand { get; private set; }
		public RelayCommand SelectNoneCommand { get; private set; }

		public override void LoadFilter(SKDReportFilter filter)
		{
			var organisationFilter = filter as IReportFilterOrganisation;
			var uids = organisationFilter == null ? null : organisationFilter.Organisations;
			if (!AllowMultiple && uids == null && _organisationsFilter.Organisations.Items.Count > 0)
				uids = new List<Guid>() { _organisationsFilter.Organisations.Items.First().Organisation.UID };
			_organisationsFilter.Initialize(uids);
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var organisationFilter = filter as IReportFilterOrganisation;
			if (organisationFilter != null)
				organisationFilter.Organisations = _organisationsFilter.UIDs;
		}
	}
}
