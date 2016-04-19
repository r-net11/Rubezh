using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.CheckBoxList;
using Infrastructure.Common.SKDReports;
using Infrastructure.Events;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;
using RubezhClient;
using RubezhClient.SKDHelpers;
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
		}

		public bool AllowMultiple { get; set; }
		public ReportOrganisationsItemList Organisations { get; private set; }
		public List<Guid> UIDs { get { return Organisations.Items.Where(x => x.IsChecked).Select(x => x.Organisation.UID).ToList(); } }
		public RelayCommand SelectAllCommand { get; private set; }
		public RelayCommand SelectNoneCommand { get; private set; }

		public override void LoadFilter(SKDReportFilter filter)
		{
			ServiceFactory.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Unsubscribe(OnUseArchive);
			ServiceFactory.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Subscribe(OnUseArchive);
			var organisationFilter = filter as IReportFilterOrganisation;
			var uids = organisationFilter == null ? null : organisationFilter.Organisations;
			if (!AllowMultiple && uids == null && Organisations.Items.Count > 0)
				uids = new List<Guid>();
			CreateItemList();
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
			var filter = new OrganisationFilter() { UserUID = ClientManager.CurrentUser.UID };
			if (isWithDeleted)
				filter.LogicalDeletationType = LogicalDeletationType.All;
			var organisations = OrganisationHelper.Get(filter);
			UpdateItemList(organisations);
		}
		void UpdateItemList(IEnumerable<Organisation> organisations)
		{
			Organisations = new ReportOrganisationsItemList { IsSingleSelection = !AllowMultiple };
			if (organisations != null)
			{
				foreach (var organisation in organisations)
				{
					Organisations.Add(new ReportFilterOrganisationViewModel(organisation));
				}
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
			{
				organisation.IsChecked = true;
			}
			if (!AllowMultiple && Organisations.Items.Count > 0 && uids.Count != 1)
				Organisations.SelectedOrganisation = Organisations.Items.FirstOrDefault();
		}

		public override void Unsubscribe()
		{
			ServiceFactory.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Unsubscribe(OnUseArchive);
		}
	}

	public class ReportOrganisationsItemList : CheckBoxItemList<ReportFilterOrganisationViewModel>
	{
		ReportFilterOrganisationViewModel _SelectedOrganisation;
		public ReportFilterOrganisationViewModel SelectedOrganisation
		{
			get { return _SelectedOrganisation; }
			set
			{
				_SelectedOrganisation = value;
				if (IsSingleSelection)
					_SelectedOrganisation.IsChecked = true;
			}
		}

		public override void Update()
		{
			base.Update();
			ServiceFactory.Events.GetEvent<SKDReportOrganisationChangedEvent>().Publish(Items.Where(x => x.IsChecked).Select(x => x.Organisation.UID).ToList());
		}
	}
}