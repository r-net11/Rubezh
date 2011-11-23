using Infrastructure.Common;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
    public class ElementViewModel : BaseViewModel
    {
        DesignerItem DesignerItem;

        public ElementViewModel(DesignerItem designerItem)
        {
            DesignerItem = designerItem;
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
