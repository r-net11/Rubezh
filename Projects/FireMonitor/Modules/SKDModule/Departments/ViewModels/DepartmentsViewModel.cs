using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using SKDModule.Common;

namespace SKDModule.ViewModels
{
	public class DepartmentsViewModel : CartothequeTabItemCopyPasteBase<ShortDepartment, DepartmentFilter, DepartmentViewModel, DepartmentDetailsViewModel>
	{
		protected override void InitializeModels(IEnumerable<ShortDepartment> models)
		{
			foreach (var organisation in Organisations)
			{
				foreach (var model in models)
				{
					if (model.OrganisationUID == organisation.Organisation.UID && model.ParentDepartmentUID == null)
					{
						var itemViewModel = new DepartmentViewModel();
						itemViewModel.InitializeModel(organisation.Organisation, model, this);
						organisation.AddChild(itemViewModel);
						AddChildren(itemViewModel, models);
					}
				}
			}
		}

		void AddChildren(DepartmentViewModel parentViewModel, IEnumerable<ShortDepartment> models)
		{
			if (parentViewModel.Model.ChildDepartmentUIDs != null && parentViewModel.Model.ChildDepartmentUIDs.Count > 0)
			{
				var children = models.Where(x => x.ParentDepartmentUID == parentViewModel.Model.UID);
				foreach (var child in children)
				{
					var itemViewModel = new DepartmentViewModel();
					itemViewModel.InitializeModel(parentViewModel.Organisation, child, this);
					parentViewModel.AddChild(itemViewModel);
					AddChildren(itemViewModel, models);
				}
			}
		}

		protected override DepartmentViewModel GetParentItem()
		{
			return SelectedItem;
		}

		protected override IEnumerable<ShortDepartment> GetModels(DepartmentFilter filter)
		{
			return DepartmentHelper.Get(filter);
		}

		protected override IEnumerable<ShortDepartment> GetModelsByOrganisation(Guid organisationUID)
		{
			return DepartmentHelper.GetByOrganisation(organisationUID);
		}

		protected override bool MarkDeleted(Guid uid)
		{
			return DepartmentHelper.MarkDeleted(uid);
		}

		protected override bool Save(ShortDepartment item)
		{
			var department = new Department()
				{
					UID = item.UID,
					Name = item.Name,
					Description = item.Description,
					ParentDepartmentUID = item.ParentDepartmentUID,
					OrganisationUID = item.OrganisationUID,
				};
			return DepartmentHelper.Save(department);
		}
         
		protected override void OnPaste()
        {
            Guid? parentDepartmentUID = null;
            if (SelectedItem.Parent != null && !SelectedItem.Parent.IsOrganisation)
                parentDepartmentUID = SelectedItem.Parent.Model.UID;
            _clipboard.ParentDepartmentUID = parentDepartmentUID;
            var newItem = CopyModel(_clipboard);
            if (Save(newItem))
            {
                var itemViewModel = new DepartmentViewModel();
                itemViewModel.InitializeModel(SelectedItem.Organisation, newItem, this);
                SelectedItem.AddChild(itemViewModel);
                SelectedItem = itemViewModel;
            }
        }

        protected override bool CanPaste()
        {
            return SelectedItem != null && _clipboard != null && ParentOrganisation != null;
        }
        
        protected override ShortDepartment CopyModel(ShortDepartment source, bool newName = true)
        {
            var copy = new ShortDepartment();
            copy.Name = newName ? CopyHelper.CopyName(source.Name, ParentOrganisation.Children.Select(item => item.Name)) : source.Name;
            copy.Description = source.Description;
            if (SelectedItem.Model != null)
                copy.ParentDepartmentUID = SelectedItem.Model.UID;
            copy.OrganisationUID = ParentOrganisation.Organisation.UID;
            return copy;
        }

        protected override string ItemRemovingName
        {
            get { return "отдел"; }
        }
	}
}