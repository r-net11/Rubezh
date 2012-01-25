using Infrastructure.Common;
using PlansModule.Designer;
using FiresecAPI.Models;
using PlansModule.Events;
using Infrastructure;

namespace PlansModule.ViewModels
{
    public class ElementViewModel : BaseViewModel
    {
        public ElementViewModel(DesignerItem designerItem, string name)
        {
            ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
            DesignerItem = designerItem;
            Name = name;
        }

        public DesignerItem DesignerItem { get; private set; }
        public string Name { get; private set; }

        public bool IsVisible
        {
            get { return DesignerItem.IsVisibleLayout; }
            set
            {
                DesignerItem.IsVisibleLayout = value;
                OnPropertyChanged("IsVisible");
            }
        }

        public bool IsSelectable
        {
            get { return DesignerItem.IsSelectableLayout; }
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
