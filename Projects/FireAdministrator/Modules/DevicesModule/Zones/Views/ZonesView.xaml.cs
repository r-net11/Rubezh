using System.Windows.Controls;

namespace DevicesModule.Views
{
	public partial class ZonesView : UserControl
	{
		public ZonesView()
		{
			InitializeComponent();
			_zones.SelectionChanged += new SelectionChangedEventHandler(_zonesListBox_SelectionChanged);
		}

		void _zonesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_zones.SelectedItem != null)
				_zones.ScrollIntoView(_zones.SelectedItems[_zones.SelectedItems.Count - 1]);
		}
	}
}