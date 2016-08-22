using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using StrazhAPI.Models;
using StrazhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class AccessTemplatesFilterViewModel : OrganisationBaseViewModel<AccessTemplate, AccessTemplateFilter, AccessTemplateFilterItemViewModel, AccessTemplateDetailsViewModel>
	{
		private bool _showCardsWithEmptyAccessTemplate;
		public bool ShowCardsWithEmptyAccessTemplate
		{
			get { return _showCardsWithEmptyAccessTemplate; }
			set
			{
				_showCardsWithEmptyAccessTemplate = value;
				OnPropertyChanged(() => ShowCardsWithEmptyAccessTemplate);
			}
		}

		public RelayCommand SelectAllCommand { get; private set; }
		void OnSelectAll()
		{
			var models = Organisations.SelectMany(x => x.GetAllChildren(false));
			foreach (var model in models)
				model.IsChecked = true;
		}

		public RelayCommand SelectNoneCommand { get; private set; }
		void OnSelectNone()
		{
			var models = Organisations.SelectMany(x => x.GetAllChildren(false));
			foreach (var model in models)
				model.IsChecked = false;
		}

		public AccessTemplatesFilterViewModel()
		{
			SelectAllCommand = new RelayCommand(OnSelectAll);
			SelectNoneCommand = new RelayCommand(OnSelectNone);
		}

		#region <OrganisationBaseViewModel>

		protected override IEnumerable<AccessTemplate> GetModels(AccessTemplateFilter filter)
		{
			return AccessTemplateHelper.Get(filter);
		}

		protected override IEnumerable<AccessTemplate> GetModelsByOrganisation(Guid organisationUID)
		{
			return AccessTemplateHelper.GetByOrganisation(organisationUID);
		}

		protected override bool MarkDeleted(AccessTemplate model)
		{
			return AccessTemplateHelper.MarkDeleted(model);
		}

		protected override bool Add(AccessTemplate item)
		{
			throw new NotImplementedException();
		}

		protected override bool Restore(AccessTemplate model)
		{
			return AccessTemplateHelper.Restore(model);
		}

		protected override PermissionType Permission
		{
			get { return StrazhAPI.Models.PermissionType.Oper_SKD_AccessTemplates_Etit; }
		}

		public override void Initialize(AccessTemplateFilter filter)
		{
			var emptyFilter = new AccessTemplateFilter
			{
				LogicalDeletationType = filter.LogicalDeletationType,
				OrganisationUIDs = filter.OrganisationUIDs
			};
			base.Initialize(filter);
			var models = Organisations.SelectMany(x => x.Children);
			SetSelected(models, filter.UIDs ?? new List<Guid>());
		}

		#endregion </OrganisationBaseViewModel>

		public void Initialize(List<Guid> organisationUiDs, LogicalDeletationType logicalDeletationType)
		{
			var filter = new AccessTemplateFilter
			{
				LogicalDeletationType = logicalDeletationType,
				OrganisationUIDs = organisationUiDs
			};
			Initialize(filter);
		}
		
		private void SetSelected(IEnumerable<AccessTemplateFilterItemViewModel> accessTemplates, List<Guid> selected)
		{
			foreach (var accessTemplate in accessTemplates)
			{
				if (selected.Contains(accessTemplate.Model.UID))
				{
					accessTemplate.IsChecked = true;
					accessTemplate.ExpandToThis();
				}
				SetSelected(accessTemplate.Children, selected);
			}
		}

		public List<Guid> UIDs { get { return GetSelected(Organisations.SelectMany(x => x.Children)).ToList(); } }

		private IEnumerable<Guid> GetSelected(IEnumerable<AccessTemplateFilterItemViewModel> accessTemplates)
		{
			foreach (var accessTemplate in accessTemplates)
			{
				if (accessTemplate.IsChecked)
					yield return accessTemplate.Model.UID;
				foreach (var subdepartment in GetSelected(accessTemplate.Children))
					yield return subdepartment;
			}
		}

	}
}