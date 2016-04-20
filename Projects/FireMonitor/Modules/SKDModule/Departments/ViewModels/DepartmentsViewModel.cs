using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows.Windows;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class DepartmentsViewModel : OrganisationBaseViewModel<ShortDepartment, DepartmentFilter, DepartmentViewModel, DepartmentDetailsViewModel>
	{
		List<ShortDepartment> _clipboardChildren;

		public DepartmentsViewModel():base()
		{
			ServiceFactory.Events.GetEvent<NewDepartmentEvent>().Unsubscribe(OnNewDepartment);
			ServiceFactory.Events.GetEvent<NewDepartmentEvent>().Subscribe(OnNewDepartment);
		}
		
		protected override void InitializeModels(IEnumerable<ShortDepartment> models)
		{
			foreach (var organisation in Organisations)
			{
				foreach (var model in models)
				{
					if (model.OrganisationUID == organisation.Organisation.UID && (model.ParentDepartmentUID == null || model.ParentDepartmentUID == Guid.Empty))
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
			if (parentViewModel.Model.ChildDepartments != null && parentViewModel.Model.ChildDepartments.Count > 0)
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

		void OnNewDepartment(ShortDepartment model)
		{
			var parent = GetParentItem(model);
			if (parent != null)
			{
				var viewModel = new DepartmentViewModel();
				viewModel.InitializeModel(parent.Organisation, model, this);
				parent.AddChild(viewModel);
			}
		}

		protected override DepartmentViewModel GetParentItem(ShortDepartment model)
		{
			return GetParentItemInternal(model);
		}

		DepartmentViewModel GetParentItemInternal(ShortDepartment model)
		{
			return Organisations.SelectMany(x => x.GetAllChildren()).FirstOrDefault(x => (model.ParentDepartmentUID != Guid.Empty && !x.IsOrganisation && x.Model.UID == model.ParentDepartmentUID) ||
				(model.ParentDepartmentUID == Guid.Empty && x.IsOrganisation && x.Organisation.UID == model.OrganisationUID));
		}

		protected override void UpdateParent()
		{
			if (SelectedItem.Model.ParentDepartmentUID != SelectedItem.Parent.UID)
			{
				var model = SelectedItem.Model;
				var newParent = GetParentItemInternal(model);
				newParent.AddChild(SelectedItem);
			}
			base.UpdateParent();
		}

		protected override void OnOrganisationUsersChanged(Organisation newOrganisation)
		{
			base.OnOrganisationUsersChanged(newOrganisation);
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
			model.ChildDepartments = new List<TinyDepartment>();
			foreach (var child in SelectedItem.GetAllChildren(false))
			{
				model.ChildDepartments.Add(new TinyDepartment { UID = child.UID, Name = child.Name });
			}
			return DepartmentHelper.MarkDeleted(model);
		}

		protected override bool Restore(ShortDepartment model)
		{
			model.ParentDepartments = new List<TinyDepartment>();
			foreach (var parent in SelectedItem.GetAllParents().Where(x => !x.IsOrganisation))
			{
				model.ParentDepartments.Add(new TinyDepartment { UID = parent.UID, Name = parent.Name });
			}
			return DepartmentHelper.Restore(model);
		}

		protected override bool Add(ShortDepartment item)
		{
			var department = DepartmentHelper.GetDetails(_clipboardUID);
			department.UID = item.UID;
			department.Name = item.Name;
			department.Description = item.Description;
			department.Phone = item.Phone;
			department.ParentDepartmentUID = item.ParentDepartmentUID;
			department.OrganisationUID = item.OrganisationUID;
			department.ChildDepartmentUIDs = item.ChildDepartments.Select(x => x.UID).ToList();
			return DepartmentHelper.Save(department, true);
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
			CopyChildren(_clipboard,  newItem, allDevices, true);
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

		protected override bool CanPaste()
		{
			return SelectedItem != null && _clipboard != null && ParentOrganisation != null && !SelectedItem.IsDeleted;
		}

		protected override void OnCopy()
		{
			base.OnCopy();
			CopyChildren(SelectedItem.Model,  _clipboard, Organisations.SelectMany(x => x.GetAllChildren(false)).Select(x => x.Model).ToList(), true);
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

		protected override void Remove()
		{
			var employeeUIDs = DepartmentHelper.GetChildEmployeeUIDs(SelectedItem.UID);
			if (employeeUIDs == null || employeeUIDs.Count() == 0 ||
				MessageBoxService.ShowQuestion("Существуют привязанные к подразделению сотрудники. Продолжить?"))
			{
				base.Remove();
				if (IsWithDeleted)
				{
					foreach (var child in SelectedItem.GetAllChildren())
					{
						child.IsDeleted = true;
						child.RemovalDate = SelectedItem.RemovalDate;
					}
				}
				foreach (var uid in employeeUIDs)
				{
					ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Publish(uid);
				}
			}
		}

		protected override void Restore()
		{
			base.Restore();
			foreach (var child in SelectedItem.GetAllParents().Where(x => x.IsDeleted))
			{
				child.IsDeleted = false;
				SelectedItem.RemovalDate = "";
			}
			var employeeUIDs = DepartmentHelper.GetParentEmployeeUIDs(SelectedItem.UID);;
			foreach (var uid in employeeUIDs)
			{
				ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Publish(uid);
			}
		}

		protected override string ItemRemovingName
		{
			get { return "подразделение"; }
		}


		protected override RubezhAPI.Models.PermissionType Permission
		{
			get { return RubezhAPI.Models.PermissionType.Oper_SKD_Departments_Etit; }
		}

		protected override void SetIsDeletedByOrganisation(DepartmentViewModel organisationViewModel)
		{
			base.SetIsDeletedByOrganisation(organisationViewModel);
			UpdateSelected();
		}

		protected override void UpdateSelected()
		{
			base.UpdateSelected();
			if (IsShowEmployeeList)
				SelectedItem.InitializeEmployeeList();
			OnPropertyChanged(() => IsShowEmployeeList);
		}

		public bool IsShowEmployeeList
		{
			get { return SelectedItem != null && !SelectedItem.IsOrganisation && ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Employees_View); }
		}

		protected override void AfterEdit(ShortDepartment model)
		{
			base.AfterEdit(model);
			var employeeUIDs = DepartmentHelper.GetEmployeeUIDs(model.UID);
			foreach (var uid in employeeUIDs)
			{
				ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Publish(uid);
			}
		}
	}
}