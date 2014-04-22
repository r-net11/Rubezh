using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.CheckBoxList;

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
			var organisations = OrganisationHelper.Get(new OrganisationFilter { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
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
				if (Filter.OrganisationUIDs.Any(x => x == organization.Organization.UID))
					organization.IsChecked = true;
			}
			if (!Organizations.Items.Any(x => x.IsChecked))
				Organizations.Items.ForEach(x => x.IsChecked = true);
		}

		public CheckBoxItemList<FilterOrganizationViewModel> Organizations { get; private set; }

		protected override bool Save()
		{
			base.Save();
			Filter.OrganisationUIDs = new List<Guid>();
			if (Organizations.HasCheckedItems)
			{
				foreach (var organization in Organizations.Items.Where(x => x.IsChecked))
				{
					Filter.OrganisationUIDs.Add(organization.Organization.UID);
				}
			}
			else
			{
				foreach (var organization in Organizations.Items)
				{
					Filter.OrganisationUIDs.Add(organization.Organization.UID);
				}
			}
			return true;
		}
	}
}