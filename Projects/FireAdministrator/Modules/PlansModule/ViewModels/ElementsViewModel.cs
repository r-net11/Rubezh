using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Services;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
	public class ElementsViewModel : BaseViewModel
	{
		public ElementsViewModel(DesignerCanvas designerCanvas)
		{
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementAdded);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementAdded);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
			DesignerCanvas = designerCanvas;

			Elements = new ObservableCollection<ElementBaseViewModel>();
			Update();
		}

		public DesignerCanvas DesignerCanvas { get; set; }
		public ObservableCollection<ElementBaseViewModel> Elements { get; set; }

		ElementBaseViewModel _selectedElement;
		public ElementBaseViewModel SelectedElement
		{
			get { return _selectedElement; }
			set
			{
				_selectedElement = value;
				OnPropertyChanged("SelectedElement");
			}
		}

		void OnPlansChanged(Guid planUID)
		{
			Update();
		}

		public void Update()
		{
			AllElements = new List<ElementViewModel>();
			Elements.Clear();
			Groups = new Dictionary<string, ElementGroupViewModel>();
			foreach (string alias in LayerGroupService.Instance)
			{
				var group = new ElementGroupViewModel(Elements, this, alias);
				Groups.Add(alias, group);
				Elements.Add(group);
			}

			foreach (var designerItem in DesignerCanvas.Items)
				AddElement(new ElementViewModel(Elements, designerItem));

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
			var elementViewModel = AllElements.FirstOrDefault(x => x.DesignerItem.Element.UID == elementUID);
			if (elementViewModel != null)
				elementViewModel.ExpantToThis();
			SelectedElement = elementViewModel;
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
			{
				elementViewModel.Parent = parentElementViewModel;
				parentElementViewModel.Children.Add(elementViewModel);
				var indexOf = Elements.IndexOf(parentElementViewModel);
				Elements.Insert(indexOf + 1, elementViewModel);
			}
			AllElements.Add(elementViewModel);
		}

		void OnElementAdded(List<ElementBase> elements)
		{
			foreach (var elementBase in elements)
			{
				var designerItem = DesignerCanvas.Items.FirstOrDefault(x => x.Element.UID == elementBase.UID);
				if (designerItem != null)
					AddElement(new ElementViewModel(Elements, designerItem));
			}
			if (elements.Count > 0)
				Select(elements[elements.Count - 1].UID);
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
						element.Parent.Children.Remove(element);
					element.Parent = null;
					Elements.Remove(element);
					AllElements.Remove(element);
				}
			}
			UpdateHasChildren();
		}

		void OnElementChanged(List<ElementBase> elements)
		{

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