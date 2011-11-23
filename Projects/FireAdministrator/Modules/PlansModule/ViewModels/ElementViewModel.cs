using Infrastructure.Common;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
    public class ElementViewModel : BaseViewModel
    {
        DesignerItem DesignerItem;

        public ElementViewModel(DesignerItem designerItem, string name)
        {
            DesignerItem = designerItem;
            Name = name;
        }

        public string Name { get; set; }

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
    }
}
