using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Infrastructure.Common;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using PlansModule.Designer.Adorners;

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
			MouseDoubleClick += (s, e) => OnShowProperties();
			IsVisibleLayout = true;
			IsSelectableLayout = true;
		}


		public RelayCommand ShowPropertiesCommand { get; private set; }
		private void OnShowProperties()
		{
			//DesignerCanvas.BeginChange();
			//
		}
	}
}