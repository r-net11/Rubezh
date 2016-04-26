using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Extensions;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class DepartmentsViewModel : OrganisationBaseViewModel<ShortDepartment, DepartmentFilter, DepartmentViewModel, DepartmentDetailsViewModel>
	{
		List<ShortDepartment> _clipboardChildren;

		public DepartmentsViewModel()
		{
			ServiceFactoryBase.Events.GetEvent<NewDepartmentEvent>().Unsubscribe(OnNewDepartment);
			ServiceFactoryBase.Events.GetEvent<NewDepartmentEvent>().Subscribe(OnNewDepartment);
		}

		protected override void InitializeModels(IEnumerable<ShortDepartment> models)
		{
			foreach (var organisation in Organisations)
			{
				foreach (var model in models)
				{
					if (model.OrganisationUID == organisation.Organisation.UID && (model.ParentDepartmentUID.IsNullOrEmpty()))
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
			if (parentViewModel.Model.ChildDepartments == null || !parentViewModel.Model.ChildDepartments.Any()) return;

			var children = models.Where(x => x.ParentDepartmentUID == parentViewModel.Model.UID);

			foreach (var child in children)
			{
				var itemViewModel = new DepartmentViewModel();
				itemViewModel.InitializeModel(parentViewModel.Organisation, child, this);
				parentViewModel.AddChild(itemViewModel);
				AddChildren(itemViewModel, models);
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
			return Organisations.SelectMany(x => x.GetAllChildren())
				.FirstOrDefault(x =>
					(!model.ParentDepartmentUID.IsNullOrEmpty() && !x.IsOrganisation && x.Model.UID == model.ParentDepartmentUID) ||
					(model.ParentDepartmentUID.IsNullOrEmpty() && x.IsOrganisation && x.Organisation.UID == model.OrganisationUID)
				);
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
			model.ChildDepartments = new Dictionary<Guid, string>();

			foreach (var child in SelectedItem.GetAllChildren(false))
			{
				model.ChildDepartments.Add(child.UID, child.Name);
			}

			return DepartmentHelper.MarkDeleted(model);
		}

		protected override bool Restore(ShortDepartment model)
		{
			model.ParentDepartments = new Dictionary<Guid, string>();

			foreach (var parent in SelectedItem.GetAllParents().Where(x => !x.IsOrganisation && x.IsDeleted))
			{
				model.ParentDepartments.Add(parent.UID, parent.Name);
			}

			return DepartmentHelper.Restore(model);
		}

		protected override bool Add(ShortDepartment item)
		{
			var department = DepartmentHelper.GetDetails(_clipboardUID);
			department.UID = item.UID;
			department.Name = item.Name;
			department.Description = item.Description;
			department.ParentDepartmentUID = item.ParentDepartmentUID;
			department.OrganisationUID = item.OrganisationUID;
			department.ChildDepartmentUIDs = item.ChildDepartments.Select(x => x.Key).ToList();

			return DepartmentHelper.Save(department, true);
		}

		protected override void OnPaste()
		{
			Guid? parentDepartmentUID = null;

			if (SelectedItem.Parent != null && !SelectedItem.Parent.IsOrganisation)
				parentDepartmentUID = SelectedItem.Parent.Model.UID;

			_clipboard.ParentDepartmentUID = parentDepartmentUID;
			var newItem = CopyModel(_clipboard);
			var newItemChildren = CopyChildren(newItem, _clipboardChildren);

			if (Add(newItem) && SaveMany(newItemChildren))
			{
				var itemViewModel = new DepartmentViewModel();
				itemViewModel.InitializeModel(SelectedItem.Organisation, newItem, this);
				SelectedItem.AddChild(itemViewModel);
				AddChildren(itemViewModel, newItemChildren);
				SelectedItem = itemViewModel;
			}
		}

		bool SaveMany(List<ShortDepartment> models)
		{
			return models.Select(Add).All(saveResult => saveResult);
		}

		protected override bool CanPaste()
		{
			return SelectedItem != null && _clipboard != null && ParentOrganisation != null && !SelectedItem.IsDeleted;
		}

		protected override void OnCopy()
		{
			base.OnCopy();
			_clipboardChildren = CopyChildren(_clipboard, SelectedItem.Children.Select(x => x.Model));
		}

		protected override ShortDepartment CopyModel(ShortDepartment source)
		{
			var copy = base.CopyModel(source);

			if (SelectedItem.Model != null)
				copy.ParentDepartmentUID = SelectedItem.Model.UID;

			return copy;
		}

		List<ShortDepartment> CopyChildren(ShortDepartment parent, IEnumerable<ShortDepartment> children)
		{
			var result = new List<ShortDepartment>();
			parent.ChildDepartments = new Dictionary<Guid,string>();

			foreach (var item in children)
			{
				var shortDepartment = base.CopyModel(item);
				shortDepartment.ParentDepartmentUID = parent.UID;
				result.Add(shortDepartment);
				parent.ChildDepartments.Add(shortDepartment.UID, shortDepartment.Name);
			}

			return result;
		}

		protected override void Remove()
		{
			var employeeUIDs = DepartmentHelper.GetChildEmployeeUIDs(SelectedItem.UID);
			if (employeeUIDs == null || !employeeUIDs.Any() ||
				MessageBoxService.ShowQuestion("Существуют привязанные к подразделению сотрудники. Продожить?"))
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
					ServiceFactoryBase.Events.GetEvent<EditEmployeeEvent>().Publish(uid);
				}
			}
		}

		protected override void Restore()
		{
			base.Restore();
			foreach (var child in SelectedItem.GetAllParents().Where(x => x.IsDeleted))
			{
				child.IsDeleted = false;
				child.RemovalDate = null;
			}
			var employeeUIDs = DepartmentHelper.GetParentEmployeeUIDs(SelectedItem.UID);
			foreach (var uid in employeeUIDs)
			{
				ServiceFactoryBase.Events.GetEvent<EditEmployeeEvent>().Publish(uid);
			}
		}

		protected override string ItemRemovingName
		{
			get { return "подразделение"; }
		}

		protected override FiresecAPI.Models.PermissionType Permission
		{
			get { return FiresecAPI.Models.PermissionType.Oper_SKD_Departments_Etit; }
		}

		protected override void SetIsDeletedByOrganisation(DepartmentViewModel organisationViewModel)
		{
			base.SetIsDeletedByOrganisation(organisationViewModel);
			UpdateSelected();
		}

		protected override void UpdateSelected()
		{
			base.UpdateSelected();
			if (EmployeeListViewModel == null)
				EmployeeListViewModel = new DepartmentEmployeeListViewModel(SelectedItem, IsWithDeleted);
			else
				EmployeeListViewModel.Initialize(SelectedItem, IsWithDeleted);
			OnPropertyChanged(() => IsShowEmployeeList);
		}

		public DepartmentEmployeeListViewModel EmployeeListViewModel { get; private set; }

		public bool IsShowEmployeeList
		{
			get { return SelectedItem != null && !SelectedItem.IsOrganisation && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_Employees_View); }
		}
	}
}