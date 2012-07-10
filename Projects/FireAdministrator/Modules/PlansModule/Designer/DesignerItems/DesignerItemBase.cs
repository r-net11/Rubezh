using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Infrastructure.Common;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using PlansModule.Designer.Adorners;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Events;

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemBase : DesignerItem
	{
		static DesignerItemBase()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
		}

		public DesignerItemBase(ElementBase element)
			: base(element)
		{
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			DeleteCommand = new RelayCommand(OnDelete);
			MouseDoubleClick += (s, e) => ShowPropertiesCommand.Execute(null);
			IsVisibleLayout = true;
			IsSelectableLayout = true;
		}

		protected override void OnShowProperties()
		{
			var property = CreatePropertiesViewModel();
			if (property != null)
			{
				DesignerCanvas.BeginChange();
				if (DialogService.ShowModalWindow(property))
				{
					Redraw();
					ServiceFactory.SaveService.PlansChanged = true;
					DesignerCanvas.EndChange();
				}
			}
		}
		protected override void OnDelete()
		{
			((DesignerCanvas)DesignerCanvas).RemoveAllSelected();
		}
		protected virtual SaveCancelDialogViewModel CreatePropertiesViewModel()
		{
			var args = new ShowPropertiesEventArgs(Element);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Publish(args);
			return args.PropertyViewModel as SaveCancelDialogViewModel;
		}
	}
}