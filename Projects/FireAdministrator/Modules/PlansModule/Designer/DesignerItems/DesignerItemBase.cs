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
			MouseDoubleClick += (s, e) => ShowPropertiesCommand.Execute();
			IsVisibleLayout = true;
			IsSelectableLayout = true;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		private void OnShowProperties()
		{
			var property = CreateProperty();
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
		protected SaveCancelDialogViewModel CreateProperty()
		{
			return null;
		}
	}
}