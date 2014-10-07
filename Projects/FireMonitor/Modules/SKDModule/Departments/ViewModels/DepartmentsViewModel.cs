using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
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

		void OnNewDepartment(ShortDepartment model)
		{
			var parent = Organisations.SelectMany(x => x.GetAllChildren()).FirstOrDefault(x => !x.IsOrganisation && x.Model.UID == model.ParentDepartmentUID);
			if (parent != null)
			{
				var viewModel = new DepartmentViewModel();
				viewModel.InitializeModel(parent.Organisation, model, this);
				parent.AddChild(viewModel);
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

		protected override bool Restore(Guid uid)
		{
			return DepartmentHelper.Restore(uid);
		}

		protected override bool Save(ShortDepartment item)
		{
			var department = DepartmentHelper.GetDetails(_clipboardUID);
			department.UID = item.UID;
			department.Name = item.Name;
			department.Description = item.Description;
			department.ParentDepartmentUID = item.ParentDepartmentUID;
			department.OrganisationUID = item.OrganisationUID;
			department.ChildDepartmentUIDs = item.ChildDepartmentUIDs;
			return DepartmentHelper.Save(department);
		}
		 
		protected override void OnPaste()
		{
			Guid? parentDepartmentUID = null;
			if (SelectedItem.Parent != null && !SelectedItem.Parent.IsOrganisation)
				parentDepartmentUID = SelectedItem.Parent.Model.UID;
			_clipboard.ParentDepartmentUID = parentDepartmentUID;
			var newItem = CopyModel(_clipboard);
			var newItemChildren = CopyChildren(newItem, _clipboardChildren);
			if (Save(newItem) && SaveMany(newItemChildren))
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
			foreach (var item in models)
			{
				var saveResult = Save(item);
				if (!saveResult)
					return false;
			}
			return true;
		}

		protected override bool CanPaste()
		{
			return SelectedItem != null && _clipboard != null && ParentOrganisation != null;
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
			parent.ChildDepartmentUIDs = new List<Guid>();
			foreach (var item in children)
			{
				var shortDepartment = base.CopyModel(item);
				shortDepartment.ParentDepartmentUID = parent.UID;
				result.Add(shortDepartment);
				parent.ChildDepartmentUIDs.Add(shortDepartment.UID);
			}
			return result;
		}

		protected override string ItemRemovingName
		{
			get { return "отдел"; }
		}
	}
}