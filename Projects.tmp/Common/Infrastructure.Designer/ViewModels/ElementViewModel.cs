using System.Windows.Controls;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Events;

namespace Infrastructure.Designer.ViewModels
{
	public class ElementViewModel : ElementBaseViewModel
	{
		public ElementViewModel(DesignerItem designerItem)
		{
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
			DesignerItem = designerItem;
			DesignerItem.IndexPropertyChanged += (s, e) =>
			{
				OnPropertyChanged(() => Index);
				OnPropertyChanged(() => ToolTip);
			};
			DesignerItem.TitleChanged += (s, e) => 
			{
				OnPropertyChanged(() => Name);
				OnPropertyChanged(() => ToolTip);
			};
			IsGroupHasChild = true;
			IconSource = DesignerItem.IconSource;
			DesignerItem.IconSourceChanged += (s, e) =>
			{
				IconSource = DesignerItem.IconSource;
				OnPropertyChanged(() => IconSource);
				OnPropertyChanged(() => ToolTip);
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
			get { return DesignerItem.IsSelectable; }
			set
			{
				DesignerItem.IsSelectable = value;
				OnPropertyChanged("IsSelectable");
				((DesignerCanvas)DesignerItem.DesignerCanvas).Toolbox.SetDefault();
			}
		}

		public override ContextMenu ContextMenu
		{
			get { return DesignerItem.GetElementContextMenu(); }
		}
		public override object ToolTip
		{
			get { return DesignerItem.ToolTip; }
		}

		void OnShowOnPlan()
		{
			if (DesignerItem.IsEnabled)
				ServiceFactoryBase.Events.GetEvent<ShowElementEvent>().Publish(DesignerItem.Element.UID);
		}
	}
}