using Infrastructure.Common;
using PlansModule.Designer;
using FiresecAPI.Models;
using PlansModule.Events;
using Infrastructure;
using System;
using System.Collections.ObjectModel;

namespace PlansModule.ViewModels
{
    public class ElementViewModel : ElementBaseViewModel
    {
        public ElementViewModel(ObservableCollection<ElementBaseViewModel> sourceElement, DesignerItem designerItem, string name, string elementType)
        {
            Source = sourceElement;
            ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
            DesignerItem = designerItem;
            ElementUID = DesignerItem.ElementBase.UID;
            Name = name;
            ElementType = elementType;
        }

        DesignerItem DesignerItem;
        public Guid ElementUID { get; private set; }
        public string Name { get; private set; }
        public string ElementType { get; private set; }

        public bool IsVisible
        {
            get
            {
                return DesignerItem.IsVisibleLayout;
            }
            set
            {
                DesignerItem.IsVisibleLayout = value;
                OnPropertyChanged("IsVisible");
            }
        }

        public bool IsSelectable
        {
            get
            {
                return DesignerItem.IsSelectableLayout;
            }
            set
            {
                DesignerItem.IsSelectableLayout = value;
                OnPropertyChanged("IsSelectable");
            }
        }

        public RelayCommand ShowOnPlanCommand { get; private set; }
        void OnShowOnPlan()
        {
            ServiceFactory.Events.GetEvent<ShowElementEvent>().Publish(DesignerItem.ElementBase.UID);
        }
    }
}