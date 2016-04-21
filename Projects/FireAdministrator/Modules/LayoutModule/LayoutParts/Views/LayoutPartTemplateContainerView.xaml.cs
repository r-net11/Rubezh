using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock;

namespace LayoutModule.LayoutParts.Views
{
	public partial class LayoutPartTemplateContainerView : UserControl
	{
		public static readonly DependencyProperty ManagerProperty = DependencyProperty.Register("Manager", typeof(DockingManager), typeof(LayoutPartTemplateContainerView), new UIPropertyMetadata(null));
		public DockingManager Manager
		{
			get { return (DockingManager)GetValue(ManagerProperty); }
			set { SetValue(ManagerProperty, value); }
		}
		public LayoutPartTemplateContainerView()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(LayoutPartTemplateContainerView_Loaded);
		}

		void LayoutPartTemplateContainerView_Loaded(object sender, RoutedEventArgs e)
		{
			var binding = new Binding("Manager") { Mode = BindingMode.OneWayToSource };
			var expression = SetBinding(ManagerProperty, binding);
			Manager = manager2;
		}
	}
}