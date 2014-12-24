using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;

namespace SKDModule.ViewModels
{
	public class DepartmentsFilterViewModel : OrganisationBaseViewModel<ShortDepartment, DepartmentFilter, DepartmentFilterItemViewModel, DepartmentDetailsViewModel>
	{
		public DepartmentsFilterViewModel()
			: base()
		{
			SelectAllCommand = new RelayCommand(OnSelectAll);
			SelectNoneCommand = new RelayCommand(OnSelectNone);
		}

		public override void Initialize(DepartmentFilter filter)
		{
			var emptyFilter = new DepartmentFilter { LogicalDeletationType = filter.LogicalDeletationType };
			base.Initialize(emptyFilter);
			if (filter.UIDs == null)
				return;
			var models = Organisations.SelectMany(x => x.Children).Where(x => filter.UIDs.Any(y => y == x.Model.UID));
			foreach (var model in models)
				model.IsChecked = true;
		}

		public void Initialize(List<Guid> uids, LogicalDeletationType logicalDeletationType = LogicalDeletationType.Active)
		{
			var filter = new DepartmentFilter { LogicalDeletationType = logicalDeletationType, UIDs = uids };
			Initialize(filter);
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

		protected override IEnumerable<ShortDepartment> GetModels(DepartmentFilter filter)
		{
			return DepartmentHelper.Get(filter);
		}
		protected override IEnumerable<ShortDepartment> GetModelsByOrganisation(Guid organisationUID)
		{
			return DepartmentHelper.GetByOrganisation(organisationUID);
		}
		protected override bool MarkDeleted(ShortDepartment model)
		{
			return DepartmentHelper.MarkDeleted(model);
		}
		protected override bool Restore(ShortDepartment model)
		{
			return DepartmentHelper.Restore(model);
		}
		protected override bool Add(ShortDepartment item)
		{
			throw new NotImplementedException();
		}

		protected override void InitializeModels(IEnumerable<ShortDepartment> models)
		{
			foreach (var organisation in Organisations)
			{
				foreach (var model in models)
				{
					if (model.OrganisationUID == organisation.Organisation.UID && (model.ParentDepartmentUID == null || model.ParentDepartmentUID == Guid.Empty))
					{
						var itemViewModel = new DepartmentFilterItemViewModel();
						itemViewModel.InitializeModel(organisation.Organisation, model, this);
						organisation.AddChild(itemViewModel);
						AddChildren(itemViewModel, models);
					}
				}
			}
		}

		void AddChildren(DepartmentFilterItemViewModel parentViewModel, IEnumerable<ShortDepartment> models)
		{
			if (parentViewModel.Model.ChildDepartmentUIDs != null && parentViewModel.Model.ChildDepartmentUIDs.Count > 0)
			{
				var children = models.Where(x => x.ParentDepartmentUID == parentViewModel.Model.UID);
				foreach (var child in children)
				{
					var itemViewModel = new DepartmentFilterItemViewModel();
					itemViewModel.InitializeModel(parentViewModel.Organisation, child, this);
					parentViewModel.AddChild(itemViewModel);
					AddChildren(itemViewModel, models);
				}
			}
		}

		public List<Guid> UIDs { get { return Organisations.SelectMany(x => x.Children).Where(x => x.IsChecked).Select(x => x.Model.UID).ToList(); } }
	}
}