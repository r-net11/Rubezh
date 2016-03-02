using System.Windows.Threading;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.CheckBoxList;
using Infrastructure.Common.Services;
using Infrastructure.Common.SKDReports;
using Infrastructure.Events;
using SKDModule.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SKDModule.Reports.ViewModels
{
	public class OrganizationPageViewModel : FilterContainerViewModel
	{
		public OrganizationPageViewModel(bool allowMultiple)
		{
			Title = "Организации";
			AllowMultiple = allowMultiple;
			CreateItemList();
			SelectAllCommand = new RelayCommand(() => Organisations.Items.ForEach(item => item.IsChecked = true));
			SelectNoneCommand = new RelayCommand(() => Organisations.Items.ForEach(item => item.IsChecked = false));
			ServiceFactoryBase.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Unsubscribe(OnUseArchive);
			ServiceFactoryBase.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Subscribe(OnUseArchive);
		}

		public bool AllowMultiple { get; set; }
		public ReportOrganisationsItemList Organisations { get; private set; }
		public List<Guid> UIDs { get { return Organisations.Items.Where(x => x.IsChecked).Select(x => x.Organisation.UID).ToList(); } }
		public RelayCommand SelectAllCommand { get; private set; }
		public RelayCommand SelectNoneCommand { get; private set; }

		public override void LoadFilter(SKDReportFilter filter)
		{
			var organisationFilter = filter as IReportFilterOrganisation;

			var uids = organisationFilter == null ? null : organisationFilter.Organisations;

			if (!AllowMultiple && uids == null && Organisations.Items.Any())
			{
				uids = new List<Guid>();
			}

			Initialize(uids);
		}

		public override void UpdateFilter(SKDReportFilter filter)
		{
			var organisationFilter = filter as IReportFilterOrganisation;
			if (organisationFilter != null)
				organisationFilter.Organisations = UIDs;
		}
		void OnUseArchive(bool isWithDeleted)
		{
			CreateItemList(isWithDeleted);
			Initialize(UIDs);
		}

		void CreateItemList(bool isWithDeleted = false)
		{
			Organisations = new ReportOrganisationsItemList { IsSingleSelection = !AllowMultiple };
			var filter = new OrganisationFilter { UserUID = FiresecManager.CurrentUser.UID };
			if (isWithDeleted)
				filter.LogicalDeletationType = LogicalDeletationType.All;
			var organisations = OrganisationHelper.Get(filter);

			if (organisations == null) return;

			Organisations = new ReportOrganisationsItemList();
			foreach (var organisation in organisations.OrderBy(x => x.Name))
			{
				Organisations.Add(new ReportFilterOrganisationViewModel(organisation));
			}
		}

		public void Initialize(List<Guid> uids)
		{
			foreach (var organisation in Organisations.Items)
				organisation.IsChecked = false;
			if (uids == null)
				return;
			var checkedOrganisations = Organisations.Items.Where(x => uids.Any(y => y == x.Organisation.UID));
			foreach (var organisation in checkedOrganisations)
				organisation.IsChecked = true;
		}

		public void CheckFirstOrganisation(SKDReportFilter filter)
		{
			var organisationFilter = filter as IReportFilterOrganisation;
			if (organisationFilter == null) return;

			var firstOrg = Organisations.Items.FirstOrDefault();
			if (firstOrg != null && filter.IsDefault && organisationFilter.Organisations == null)
			{
				Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => Initialize(new List<Guid> {firstOrg.Organisation.UID})));
			}
		}
	}

	public class ReportOrganisationsItemList : CheckBoxItemList<ReportFilterOrganisationViewModel>
	{
		public override void Update()
		{
			base.Update();
			ServiceFactoryBase.Events.GetEvent<SKDReportOrganisationChangedEvent>().Publish(Items.Where(x => x.IsChecked).Select(x => x.Organisation.UID).ToList());
		}
	}
}
