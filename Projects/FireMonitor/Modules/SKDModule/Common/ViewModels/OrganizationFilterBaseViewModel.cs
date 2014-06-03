using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.CheckBoxList;

namespace SKDModule.ViewModels
{
	public class OrganisationFilterBaseViewModel<T> : FilterBaseViewModel<T>
		where T : OrganisationFilterBase
	{
		public CheckBoxItemList<FilterOrganisationViewModel> Organisations { get; private set; }

		public OrganisationFilterBaseViewModel(T filter)
			: base(filter)
		{
			var organisations = OrganisationHelper.GetByCurrentUser();
			Organisations = new CheckBoxItemList<FilterOrganisationViewModel>();
			if (organisations != null)
			{
				foreach (var organisation in organisations)
				{
					Organisations.Add(new FilterOrganisationViewModel(organisation));
				}
			}

			foreach (var organisation in Organisations.Items)
			{
				organisation.IsChecked = Filter.OrganisationUIDs.Any(x => x == organisation.Organisation.UID);
			}
		}

		protected override bool Save()
		{
			base.Save();
			Filter.OrganisationUIDs = new List<Guid>();
			foreach (var organisation in Organisations.Items)
			{
				if (organisation.IsChecked)
				{
					Filter.OrganisationUIDs.Add(organisation.Organisation.UID);
				}
			}
			return true;
		}
	}
}