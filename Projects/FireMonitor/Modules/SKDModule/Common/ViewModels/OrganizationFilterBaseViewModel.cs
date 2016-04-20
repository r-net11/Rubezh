using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows.CheckBoxList;

namespace SKDModule.ViewModels
{
	public class OrganisationFilterBaseViewModel<T> : FilterBaseViewModel<T>
		where T : OrganisationFilterBase
	{
		public CheckBoxItemList<FilterOrganisationViewModel> Organisations { get; private set; }

		public OrganisationFilterBaseViewModel(T filter)
			: base(filter)
		{
			InitializeOrganisations(filter);
		}

		public void InitializeOrganisations(T filter)
		{
			var organisations = OrganisationHelper.GetByCurrentUser(filter.LogicalDeletationType);
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

			OnPropertyChanged(() => Organisations);
		}

		public List<Guid> OrganisationUIDs { get { return Organisations.Items.Where(x => x.IsChecked).Select(x => x.Organisation.UID).ToList(); } }

		protected override bool Save()
		{
			base.Save();
			Filter.OrganisationUIDs = OrganisationUIDs;
			return true;
		}
	}
}