using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.CheckBoxList;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ReportOrganisationsFilterViewModel : BaseViewModel
	{
		public CheckBoxItemList<ReportFilterOrganisationViewModel> Organisations { get; private set; }
		public List<Guid> UIDs { get { return Organisations.Items.Where(x => x.IsChecked).Select(x => x.Organisation.UID).ToList(); } }

		public ReportOrganisationsFilterViewModel(bool isWithDeleted = false)
		{
			Organisations = new CheckBoxItemList<ReportFilterOrganisationViewModel>();
			var filter = new OrganisationFilter() { UserUID = FiresecManager.CurrentUser.UID };
			if (isWithDeleted)
				filter.LogicalDeletationType = LogicalDeletationType.All;
			var organisations = OrganisationHelper.Get(filter);
			if (organisations != null)
			{
				Organisations = new CheckBoxItemList<ReportFilterOrganisationViewModel>();
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
				organisation.IsChecked = true;
		}
	}
}
