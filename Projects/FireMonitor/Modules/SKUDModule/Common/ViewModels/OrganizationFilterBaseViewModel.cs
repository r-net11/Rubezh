using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using Infrastructure.Common.CheckBoxList;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class OrganizationFilterBaseViewModel<T>:FilterBaseViewModel<T>
		where T : OrganizationFilterBase
	{
		public OrganizationFilterBaseViewModel(T filter): base(filter)
		{
			var organizations = FiresecManager.GetOrganizations(null);
			Organizations = new CheckBoxItemList<FilterOrganizationViewModel>();
			foreach (var organization in organizations)
			{
				Organizations.Add(new FilterOrganizationViewModel(organization));
			}

			foreach (var organization in Organizations.Items)
			{
				if (filter.OrganizationUids.Any(x => x == organization.Organization.UID))
					organization.IsChecked = true;
			}
		}

		public CheckBoxItemList<FilterOrganizationViewModel> Organizations { get; private set; }

		protected override bool Save()
		{
			base.Save();
			Filter.OrganizationUids = new List<Guid>();
			foreach (var organization in Organizations.Items)
			{
				if (organization.IsChecked)
					Filter.OrganizationUids.Add(organization.Organization.UID);
			};
			return true;
		}
	}
}
