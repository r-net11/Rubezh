using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock;
using System.Windows.Controls;

namespace LayoutModule.Views
{
	public partial class LayoutDesignerView : UserControl
	{
		public static readonly DependencyProperty ManagerProperty = DependencyProperty.Register("Manager", typeof(DockingManager), typeof(LayoutDesignerView), new UIPropertyMetadata(null));
		public DockingManager Manager
		{
			get { return (DockingManager)GetValue(ManagerProperty); }
			set { SetValue(ManagerProperty, value); }
		}

		public LayoutDesignerView()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(LayoutDesignerView_Loaded);
		}

		private void LayoutDesignerView_Loaded(object sender, RoutedEventArgs e)
		{
			var binding = new Binding("Manager") { Mode = BindingMode.OneWayToSource };
			var expression = SetBinding(ManagerProperty, binding);
			Manager = manager;
		}
	}
}
