using System.Collections.Generic;
using System.Windows.Controls;
using FiresecClient;

namespace DiagnosticsModule.Views
{
    public partial class DiagnosticsView : UserControl
    {
        public DiagnosticsView()
        {
            InitializeComponent();
        }

		private void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
		{
			var zones = new List<string>();
			foreach (var zone in FiresecManager.Zones)
			{
				zones.Add(zone.Name);
			}
			AutoCompleteBox autoCompleteBox = sender as AutoCompleteBox;
			autoCompleteBox.ItemsSource = zones;
		}
    }
}
