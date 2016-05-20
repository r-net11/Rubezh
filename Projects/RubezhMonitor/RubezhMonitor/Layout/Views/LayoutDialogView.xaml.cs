using RubezhMonitor.Layout.ViewModels;
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

namespace RubezhMonitor.Layout.Views
{
	public partial class LayoutDialogView : UserControl
	{
		public static readonly DependencyProperty ManagerProperty = DependencyProperty.Register("Manager", typeof(DockingManager), typeof(LayoutDialogView), new UIPropertyMetadata(null));
		public DockingManager Manager
		{
			get { return (DockingManager)GetValue(ManagerProperty); }
			set { SetValue(ManagerProperty, value); }
		}
		public LayoutDialogView()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(LayoutDialogView_Loaded);
		}

		private void LayoutDialogView_Loaded(object sender, RoutedEventArgs e)
		{
			var layoutDialogViewModel = DataContext as LayoutDialogViewModel;
			if (layoutDialogViewModel != null)
			{
				if (layoutDialogViewModel.Data.CustomPosition)
				{
					layoutDialogViewModel.Surface.WindowStartupLocation = WindowStartupLocation.Manual;
					layoutDialogViewModel.Surface.Left = layoutDialogViewModel.Data.Left;
					layoutDialogViewModel.Surface.Top = layoutDialogViewModel.Data.Top;
				}
			}

			if (Manager == null)
			{
				var binding = new Binding("LayoutContainer.Manager") { Mode = BindingMode.OneWayToSource };
				var expression = SetBinding(ManagerProperty, binding);
				Manager = manager;
			}
		}
	}
}