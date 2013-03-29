using System.Windows.Controls;
using FiresecClient;
using System.Collections.Generic;

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

		void Name_Populating(object sender, PopulatingEventArgs e)
		{
			var result = new List<string>();
			foreach (var zone in FiresecManager.Zones)
			{
				result.Add(zone.Name);
			}
			AutoCompleteBox autoCompleteBox = sender as AutoCompleteBox;
			if (autoCompleteBox != null)
			{
				autoCompleteBox.ItemsSource = result;
			}
		}

		void Description_Populating(object sender, PopulatingEventArgs e)
		{
			var result = new List<string>();
			foreach (var zone in FiresecManager.Zones)
			{
				result.Add(zone.Description);
			}
			AutoCompleteBox autoCompleteBox = sender as AutoCompleteBox;
			if (autoCompleteBox != null)
			{
				autoCompleteBox.ItemsSource = result;
			}
		}
	}
}