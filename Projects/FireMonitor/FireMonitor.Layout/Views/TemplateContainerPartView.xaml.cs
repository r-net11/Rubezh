using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock;

namespace FireMonitor.Layout.Views
{
	/// <summary>
	/// Interaction logic for TemplateContainerPartView.xaml
	/// </summary>
	public partial class TemplateContainerPartView : UserControl
	{
		public static readonly DependencyProperty ManagerProperty = DependencyProperty.Register("Manager", typeof(DockingManager), typeof(TemplateContainerPartView), new UIPropertyMetadata(null));
		public DockingManager Manager
		{
			get { return (DockingManager)GetValue(ManagerProperty); }
			set { SetValue(ManagerProperty, value); }
		}
		public TemplateContainerPartView()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(TemplateContainerPartView_Loaded);
		}

		private void TemplateContainerPartView_Loaded(object sender, RoutedEventArgs e)
		{
			var binding = new Binding("Manager") { Mode = BindingMode.OneWayToSource };
			var expression = SetBinding(ManagerProperty, binding);
			Manager = manager;
		}
	}
}
