using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.Common;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Events;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
	public class ElementViewModel : ElementBaseViewModel
	{
		public ElementViewModel(ObservableCollection<ElementBaseViewModel> sourceElement, DesignerItem designerItem)
		{
			Source = sourceElement;
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
			DesignerItem = designerItem;
			DesignerItem.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "Title")
					OnPropertyChanged("Name");
			};
		}

		public DesignerItem DesignerItem { get; private set; }
		public string Name { get { return DesignerItem.Title; } }

		public bool IsVisible
		{
			get { return DesignerItem.IsVisibleLayout; }
			set
			{
				DesignerItem.IsVisibleLayout = value;
				OnPropertyChanged("IsVisible");
				((DesignerCanvas)DesignerItem.DesignerCanvas).Toolbox.SetDefault();
			}
		}

		public bool IsSelectable
		{
			get { return DesignerItem.IsSelectableLayout; }
			set
			{
				DesignerItem.IsSelectableLayout = value;
				OnPropertyChanged("IsSelectable");
				((DesignerCanvas)DesignerItem.DesignerCanvas).Toolbox.SetDefault();
			}
		}

		void OnShowOnPlan()
		{
			if (DesignerItem.IsSelectable)
				ServiceFactory.Events.GetEvent<ShowElementEvent>().Publish(DesignerItem.Element.UID);
		}
	}
}