using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Events;
using Infrastructure.Plans.Services;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Infrastructure.Plans.ViewModels
{
	public class ElementsViewModel : BaseViewModel
	{
		public ElementsViewModel(BaseDesignerCanvas designerCanvas)
		{
			ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementAdded);
			ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementRemoved);
			ServiceFactoryBase.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
			DesignerCanvas = designerCanvas;

			Elements = new ObservableCollection<ElementBaseViewModel>();
			Update();
		}

		public BaseDesignerCanvas DesignerCanvas { get; set; }
		public ObservableCollection<ElementBaseViewModel> Elements { get; set; }

		ElementBaseViewModel _selectedElement;
		public ElementBaseViewModel SelectedElement
		{
			get { return _selectedElement; }
			set
			{
				_selectedElement = value;
				var elementViewModel = SelectedElement as ElementViewModel;
				if (elementViewModel != null && (DesignerCanvas.SelectedItems.Count() != 1 || !elementViewModel.DesignerItem.IsSelected))
					_selectedElement.ShowOnPlanCommand.Execute();
				OnPropertyChanged(() => SelectedElement);
			}
		}

		public void Update()
		{
			AllElements = new List<ElementViewModel>();
			Elements.Clear();
			Groups = new Dictionary<string, ElementGroupViewModel>();
			foreach (string alias in LayerGroupService.Instance)
			{
				var group = new ElementGroupViewModel(this, alias);
				Groups.Add(alias, group);
				Elements.Add(group);
			}

			foreach (var designerItem in DesignerCanvas.Items)
				AddElement(new ElementViewModel(designerItem));

			foreach (var group in Groups.Values)
			{
				CollapseChild(group);
				ExpandChild(group);
			}
		}

		#region ElementSelection

		public List<ElementViewModel> AllElements;
		public Dictionary<string, ElementGroupViewModel> Groups;

		public void Select(Guid elementUID)
		{
			var elementViewModel = SelectedElement as ElementViewModel;
			if (elementViewModel == null || elementViewModel.DesignerItem.Element.UID != elementUID)
			{
				elementViewModel = AllElements.FirstOrDefault(x => x.DesignerItem.Element.UID == elementUID);
				SelectedElement = elementViewModel;
			}
		}

		#endregion

		void UpdateHasChildren()
		{
			foreach (var group in Groups.Values)
				group.OnPropertyChanged("HasChildren");
		}

		void AddElement(ElementViewModel elementViewModel)
		{
			ElementGroupViewModel parentElementViewModel = Groups.ContainsKey(elementViewModel.DesignerItem.Group) ? Groups[elementViewModel.DesignerItem.Group] : null;
			if (parentElementViewModel == null)
				Elements.Add(elementViewModel);
			else
				parentElementViewModel.AddChild(elementViewModel);
			AllElements.Add(elementViewModel);
		}

		void OnElementAdded(List<ElementBase> elements)
		{
			foreach (var elementBase in elements)
			{
				var designerItem = DesignerCanvas.GetDesignerItem(elementBase.UID);
				if (designerItem != null)
					AddElement(new ElementViewModel(designerItem));
			}
			if (elements.Count > 0)
				OnElementSelected(elements[elements.Count - 1]);
			UpdateHasChildren();
		}
		void OnElementRemoved(List<ElementBase> elements)
		{
			foreach (var elementBase in elements)
			{
				var element = AllElements.FirstOrDefault(x => x.DesignerItem.Element.UID == elementBase.UID);
				if (element != null)
				{
					if ((element.Parent != null) && (element.Parent.Children != null))
						element.Parent.RemoveChild(element);
					Elements.Remove(element);
					AllElements.Remove(element);
				}
			}
			UpdateHasChildren();
		}
		void OnElementSelected(ElementBase element)
		{
			Select(element.UID);
		}

		void CollapseChild(ElementBaseViewModel parentElementViewModel)
		{
			parentElementViewModel.IsExpanded = false;

			foreach (var elementViewModel in parentElementViewModel.Children)
			{
				CollapseChild(elementViewModel);
			}
		}
		void ExpandChild(ElementBaseViewModel parentElementViewModel)
		{
			parentElementViewModel.IsExpanded = true;
			foreach (var elementViewModel in parentElementViewModel.Children)
			{
				ExpandChild(elementViewModel);
			}
		}
	}
}