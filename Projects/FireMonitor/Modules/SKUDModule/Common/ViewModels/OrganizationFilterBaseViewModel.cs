using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.CheckBoxList;

namespace SKDModule.ViewModels
{
	public class OrganisationFilterBaseViewModel<T> : FilterBaseViewModel<T>
		where T : OrganisationFilterBase
	{
		public OrganisationFilterBaseViewModel(T filter)
			: base(filter)
		{

		}

		protected override void Initialize()
		{
			base.Initialize();
			var organisations = OrganisationHelper.Get(new OrganisationFilter { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			Organisations = new CheckBoxItemList<FilterOrganisationViewModel>();
			if (organisations != null)
			{
				foreach (var organisation in organisations)
				{
					Organisations.Add(new FilterOrganisationViewModel(organisation));
				}
			}
		}

		protected override void Update()
		{
			base.Update();
			foreach (var organisation in Organisations.Items)
			{
				if (Filter.OrganisationUIDs.Any(x => x == organisation.Organisation.UID))
					organisation.IsChecked = true;
			}
			if (!Organisations.Items.Any(x => x.IsChecked))
				Organisations.Items.ForEach(x => x.IsChecked = true);
		}

		public CheckBoxItemList<FilterOrganisationViewModel> Organisations { get; private set; }

		protected override bool Save()
		{
			base.Save();
			Filter.OrganisationUIDs = new List<Guid>();
			if (Organisations.HasCheckedItems)
			{
				foreach (var organization in Organisations.Items.Where(x => x.IsChecked))
				{
					Filter.OrganisationUIDs.Add(organization.Organisation.UID);
				}
			}
			else
			{
				foreach (var organization in Organisations.Items)
				{
					Filter.OrganisationUIDs.Add(organization.Organisation.UID);
				}
			}
			return true;
		}
	}
}