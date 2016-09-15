using Infrastructure.Common.CheckBoxList;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SKDModule.ViewModels
{
	public class OrganisationFilterBaseViewModel<T> : FilterBaseViewModel<T>
		where T : OrganisationFilterBase
	{
		private CheckBoxItemList<FilterOrganisationItem> _organisations;
		private readonly object _syncObject = new object();

		public CheckBoxItemList<FilterOrganisationItem> Organisations
		{
			get { return _organisations; }
			set
			{
				_organisations = value;
				OnPropertyChanged(() => Organisations);
			}
		}

		public OrganisationFilterBaseViewModel(T filter, bool allowMultiple = true, IOrganisationsProvider organisationsProvider = null)
			: base(filter)
		{
			UpdateOrganisations(organisationsProvider, allowMultiple);
		}

		private void InitializeUI(IEnumerable<Organisation> organisations, bool allowMultiple)
		{
			lock (_syncObject)
			{
				Organisations = new CheckBoxItemList<FilterOrganisationItem> {IsSingleSelection = !allowMultiple};
				foreach (var organisation in organisations)
				{
					var tmp = organisation;
					Organisations.Add(new FilterOrganisationItem(tmp));
				}

				foreach (var organisation in Organisations.Items)
				{
					organisation.IsChecked = Filter.OrganisationUIDs.Any(x => x == organisation.Organisation.UID);

					if (organisation.IsChecked && Organisations.IsSingleSelection)
						break;
				}
			}
		}

		private static Task<IEnumerable<Organisation>> GetOrganisationsFromServer(IOrganisationsProvider organisationsProvider)
		{
			return organisationsProvider == null
				? TaskEx.Run(() => new CurrentUserOrganisationsProvider().Get())
				: TaskEx.Run(() => organisationsProvider.Get());
		}

		public void UpdateOrganisations(IOrganisationsProvider organisationsProvider, bool allowMultiple = true)
		{
			GetOrganisationsFromServer(organisationsProvider)
				.ContinueWith(t => InitializeUI(t.Result, allowMultiple), TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		protected override bool Save()
		{
			base.Save();
			Filter.OrganisationUIDs = new List<Guid>();
			foreach (var organisation in Organisations.Items.Where(organisation => organisation.IsChecked))
			{
				Filter.OrganisationUIDs.Add(organisation.Organisation.UID);
			}
			return true;
		}
	}
}