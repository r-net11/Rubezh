using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.CheckBoxList;

namespace SKDModule.ViewModels
{
	public class OrganisationFilterBaseViewModel<T> : FilterBaseViewModel<T>
		where T : OrganisationFilterBase
	{
		public CheckBoxItemList<FilterOrganisationItem> Organisations { get; private set; }

		public OrganisationFilterBaseViewModel(T filter, bool allowMultiple = true)
			: base(filter)
		{
			var organisations = OrganisationHelper.GetByCurrentUser();
			Organisations = new CheckBoxItemList<FilterOrganisationItem> { IsSingleSelection = !allowMultiple };
			if (organisations != null)
			{
				foreach (var organisation in organisations)
				{
					Organisations.Add(new FilterOrganisationItem(organisation));
				}
			}

			foreach (var organisation in Organisations.Items)
			{
				organisation.IsChecked = Filter.OrganisationUIDs.Any(x => x == organisation.Organisation.UID);

				if (organisation.IsChecked && Organisations.IsSingleSelection)
					break;
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