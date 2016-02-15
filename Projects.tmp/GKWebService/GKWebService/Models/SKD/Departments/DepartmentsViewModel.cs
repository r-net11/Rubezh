using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GKWebService.DataProviders.SKD;
using GKWebService.Models.SKD.Common;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.Models.SKD.Departments
{
    public class DepartmentsViewModel : OrganisationBaseViewModel<ShortDepartment, DepartmentFilter, DepartmentViewModel>
    {
        List<ShortDepartment> _clipboardChildren;

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
                    itemViewModel.ParentUID = parentViewModel.UID;
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
            return DepartmentHelper.Get(filter);
        }

/*
        protected override bool Add(ShortDepartment item)
        {
            var department = ClientManager.FiresecService.GetDepartmentDetails(_clipboardUID).Result;
            department.UID = item.UID;
            department.Name = item.Name;
            department.Description = item.Description;
            department.Phone = item.Phone;
            department.ParentDepartmentUID = item.ParentDepartmentUID;
            department.OrganisationUID = item.OrganisationUID;
            department.ChildDepartmentUIDs = item.ChildDepartments.Select(x => x.UID).ToList();
            return ClientManager.FiresecService.SaveDepartment(department, true).Result; 
        }

        protected override void OnPaste()
        {
            var parentDepartmentUID = Guid.Empty;
            if (SelectedItem.Parent != null && !SelectedItem.Parent.IsOrganisation)
                parentDepartmentUID = SelectedItem.Parent.Model.UID;
            _clipboard.ParentDepartmentUID = parentDepartmentUID;
            var newItem = CopyModel(_clipboard);
            var allDevices = new List<ShortDepartment>(_clipboardChildren);
            allDevices.AddRange(Organisations.SelectMany(x => x.GetAllChildren(false)).Select(x => x.Model));
            CopyChildren(_clipboard, newItem, allDevices, true);
            if (Add(newItem) && SaveMany(_clipboardChildren))
            {
                var itemViewModel = new DepartmentViewModel();
                itemViewModel.InitializeModel(SelectedItem.Organisation, newItem, this);
                SelectedItem.AddChild(itemViewModel);
                AddChildren(itemViewModel, _clipboardChildren);
                SelectedItem = itemViewModel;
                _clipboard = CopyModel(SelectedItem.Model);
                CopyChildren(SelectedItem.Model, _clipboard, Organisations.SelectMany(x => x.GetAllChildren(false)).Select(x => x.Model).ToList(), true);
            }
        }

        bool SaveMany(List<ShortDepartment> models)
        {
            foreach (var item in models)
            {
                var saveResult = Add(item);
                if (!saveResult)
                    return false;
            }
            return true;
        }

        protected override void OnCopy()
        {
            base.OnCopy();
            CopyChildren(SelectedItem.Model, _clipboard, Organisations.SelectMany(x => x.GetAllChildren(false)).Select(x => x.Model).ToList(), true);
        }

        protected override ShortDepartment CopyModel(ShortDepartment source)
        {
            var copy = base.CopyModel(source);
            copy.Phone = source.Phone;
            if (SelectedItem.Model != null)
                copy.ParentDepartmentUID = SelectedItem.Model.UID;
            return copy;
        }

        void CopyChildren(ShortDepartment parent, ShortDepartment newParent, List<ShortDepartment> allDevices, bool isClear = false)
        {
            if (isClear)
                _clipboardChildren = new List<ShortDepartment>();
            var children = allDevices.Where(x => x.ParentDepartmentUID == parent.UID);
            foreach (var child in children)
            {
                var newChild = base.CopyModel(child);
                newChild.ParentDepartmentUID = newParent.UID;
                _clipboardChildren.Add(newChild);
                newParent.ChildDepartments.Add(new TinyDepartment { UID = newChild.UID, Name = newChild.Name });
                CopyChildren(child, newChild, allDevices);
            }
        }
*/
    }
}