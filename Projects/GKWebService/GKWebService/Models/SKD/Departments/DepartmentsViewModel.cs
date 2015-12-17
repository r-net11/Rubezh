using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GKWebService.Models.SKD.Common;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.Models.SKD.Departments
{
    public class DepartmentsViewModel : OrganisationBaseViewModel<ShortDepartment, DepartmentFilter, DepartmentViewModel>
    {
        protected override void InitializeModels(IEnumerable<ShortDepartment> models)
        {
            for (int index = 0; index < Organisations.Count; index++)
            {
                var organisation = Organisations[index];
                foreach (var model in models)
                {
                    if (model.OrganisationUID == organisation.Organisation.UID &&
                        (model.ParentDepartmentUID == null || model.ParentDepartmentUID == Guid.Empty))
                    {
                        var itemViewModel = new DepartmentViewModel();
                        itemViewModel.InitializeModel(organisation.Organisation, model);
                        itemViewModel.Level = 1;
                        itemViewModel.ParentUID = itemViewModel.OrganisationUID;
                        itemViewModel.IsLeaf = true;
                        organisation.IsLeaf = false;
                        Organisations.Insert(index + 1, itemViewModel);
                        index++;
                        AddChildren(itemViewModel, models, ref index, 2);
                        itemViewModel.IsExpanded = !itemViewModel.IsLeaf;   // если был добавлен дочерний элемент, то разворачиваем
                    }
                }
            }
        }

        void AddChildren(DepartmentViewModel parentViewModel, IEnumerable<ShortDepartment> models, ref int index, int level)
        {
            if (parentViewModel.Model.ChildDepartments != null && parentViewModel.Model.ChildDepartments.Count > 0)
            {
                var children = models.Where(x => x.ParentDepartmentUID == parentViewModel.Model.UID).ToList();
                foreach (var child in children)
                {
                    var itemViewModel = new DepartmentViewModel();
                    itemViewModel.InitializeModel(parentViewModel.Organisation, child);
                    itemViewModel.Level = level;
                    itemViewModel.ParentUID = itemViewModel.OrganisationUID;
                    itemViewModel.IsLeaf = true;
                    parentViewModel.IsLeaf = false;
                    Organisations.Insert(index + 1, itemViewModel);
                    index++;
                    AddChildren(itemViewModel, models, ref index, level + 1);
                    itemViewModel.IsExpanded = !itemViewModel.IsLeaf;   // если был добавлен дочерний элемент, то разворачиваем
                }
            }
        }

        protected override IEnumerable<ShortDepartment> GetModels(DepartmentFilter filter)
        {
            return ClientManager.FiresecService.GetDepartmentList(filter).Result;
        }
    }
}