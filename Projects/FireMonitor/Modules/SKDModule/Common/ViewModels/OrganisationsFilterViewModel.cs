using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.CheckBoxList;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{

	public class OrganisationsFilterViewModel : BaseViewModel
	{
		public CheckBoxItemList<FilterOrganisationViewModel> Organisations { get; private set; }
		public List<Guid> UIDs { get { return Organisations.Items.Where(x => x.IsChecked).Select(x => x.Organisation.UID).ToList(); } }

		public OrganisationsFilterViewModel()
		{
			Organisations = new CheckBoxItemList<FilterOrganisationViewModel>();
			var organisations = OrganisationHelper.GetByCurrentUser();
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
