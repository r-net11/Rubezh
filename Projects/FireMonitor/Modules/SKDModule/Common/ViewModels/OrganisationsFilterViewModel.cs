using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows.CheckBoxList;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationsFilterViewModel : BaseViewModel
	{
		public CheckBoxItemList<FilterOrganisationViewModel> Organisations { get; private set; }
		public List<Guid> UIDs { get { return Organisations.Items.Where(x => x.IsChecked).Select(x => x.Organisation.UID).ToList(); } }
		
		public OrganisationsFilterViewModel(bool isWithDeleted = false)
		{
			Organisations = new CheckBoxItemList<FilterOrganisationViewModel>();
			var filter = new OrganisationFilter() { UserUID = ClientManager.CurrentUser.UID };
			if (isWithDeleted)
				filter.LogicalDeletationType = LogicalDeletationType.All;
			var organisations = OrganisationHelper.Get(filter);
			if (organisations != null)
			{
				Organisations = new CheckBoxItemList<FilterOrganisationViewModel>();
				foreach (var organisation in organisations)
				{
					Organisations.Add(new FilterOrganisationViewModel(organisation));
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
