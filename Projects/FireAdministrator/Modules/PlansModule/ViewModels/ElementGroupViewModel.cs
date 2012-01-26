using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PlansModule.ViewModels
{
    public class ElementGroupViewModel : ElementBaseViewModel
    {
        public ElementGroupViewModel(ObservableCollection<ElementBaseViewModel> sourceElements, ElementsViewModel elementsViewModel, string name, string elementType)
        {
            Source = sourceElements;
            IsBold = true;
            ElementsViewModel = elementsViewModel;
            Name = name;
            ElementType = elementType;
            _isVisible = true;
            _isSelectable = true;
        }

        ElementsViewModel ElementsViewModel;
        public string Name { get; private set; }
        public string ElementType { get; private set; }

        bool _isVisible;
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
                foreach (var elementViewModel in ElementsViewModel.AllElements)
                {
                    if (elementViewModel.ElementType == ElementType)
                        elementViewModel.IsVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }

        bool _isSelectable;
        public bool IsSelectable
        {
            get
            {
                return _isSelectable;
            }
            set
            {
                _isSelectable = value;
                foreach (var elementViewModel in ElementsViewModel.AllElements)
                {
                    if (elementViewModel.ElementType == ElementType)
                        elementViewModel.IsSelectable = value;
                }
                OnPropertyChanged("IsSelectable");
            }
        }
    }
}