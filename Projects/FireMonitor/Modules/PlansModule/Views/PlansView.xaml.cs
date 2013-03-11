using System.Windows.Controls;
using Infrastructure.Common;

namespace PlansModule.Views
{
	public partial class PlansView : UserControl
	{
		public PlansView()
		{
			InitializeComponent();
			Loaded += new System.Windows.RoutedEventHandler(PlansView_Loaded);
		}

		void PlansView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			//_column.SetValue(WidthProperty, (double)800);
			//_grid.Width = RegistrySettingsHelper.GetDouble("Monitor.PlansTree.Width");
		}

		private void Grid_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
		{
			//var width = e.NewSize.Width;
			//RegistrySettingsHelper.SetDouble("Monitor.PlansTree.Width", width);
		}
	}
}