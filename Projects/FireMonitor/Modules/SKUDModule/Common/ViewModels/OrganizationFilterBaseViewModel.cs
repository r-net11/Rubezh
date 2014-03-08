using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using Infrastructure.Common.CheckBoxList;
using FiresecClient;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class OrganizationFilterBaseViewModel<T> : FilterBaseViewModel<T>
		where T : OrganizationFilterBase
	{
		public OrganizationFilterBaseViewModel(T filter)
			: base(filter)
		{
			
		}

		protected override void Initialize()
		{
			base.Initialize();
			var organisations = OrganizationHelper.Get(new OrganizationFilter { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			Organizations = new CheckBoxItemList<FilterOrganizationViewModel>();
			if (organisations != null)
			{
				foreach (var organisation in organisations)
				{
					Organizations.Add(new FilterOrganizationViewModel(organisation));
				}
			}
		}

		protected override void Update()
		{
			base.Update();
			foreach (var organization in Organizations.Items)
			{
				if (Filter.OrganizationUids.Any(x => x == organization.Organization.UID))
					organization.IsChecked = true;
			}
		}

		public CheckBoxItemList<FilterOrganizationViewModel> Organizations { get; private set; }

		protected override bool Save()
		{
			base.Save();
			Filter.OrganizationUids = new List<Guid>();
			if (Organizations.HasCheckedItems)
			{
				foreach (var organization in Organizations.Items.Where(x => x.IsChecked))
				{
					Filter.OrganizationUids.Add(organization.Organization.UID);
				}
			}
			else
			{
				foreach (var organization in Organizations.Items)
				{
					Filter.OrganizationUids.Add(organization.Organization.UID);
				}
			}
			return true;
		}
	}
}