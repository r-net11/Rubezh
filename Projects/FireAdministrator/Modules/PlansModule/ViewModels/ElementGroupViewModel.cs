using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;

namespace PlansModule.ViewModels
{
    public class ElementGroupViewModel : BaseViewModel
    {
        public ElementGroupViewModel()
        {
            _isAllVisible = true;
            _isAllSelectable = true;
            Elements = new ObservableCollection<ElementViewModel>();
        }

        public string Name { get; set; }
        public ObservableCollection<ElementViewModel> Elements { get; set; }

        ElementViewModel _selectedElement;
        public ElementViewModel SelectedElement
        {
            get { return _selectedElement; }
            set
            {
                _selectedElement = value;
                OnPropertyChanged("SelectedElement");
            }
        }

        bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        bool _isAllVisible;
        public bool IsAllVisible
        {
            get { return _isAllVisible; }
            set
            {
                _isAllVisible = value;
                foreach (var device in Elements)
                    device.IsVisible = value;
                OnPropertyChanged("IsAllVisible");
            }
        }

        bool _isAllSelectable;
        public bool IsAllSelectable
        {
            get { return _isAllSelectable; }
            set
            {
                _isAllSelectable = value;
                foreach (var device in Elements)
                    device.IsSelectable = value;
                OnPropertyChanged("IsAllSelectable");
            }
        }

        public bool HasElements
        {
            get { return Elements.Count > 0; }
        }

        public void UpdateHasElements()
        {
            OnPropertyChanged("HasElements");
        }
    }
}
