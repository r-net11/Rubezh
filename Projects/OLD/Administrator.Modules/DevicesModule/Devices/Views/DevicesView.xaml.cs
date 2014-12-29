using System.Collections.Generic;
using System.Windows.Controls;
using FiresecClient;

namespace DevicesModule.Views
{
	public partial class DevicesView : UserControl
	{
		public DevicesView()
		{
			InitializeComponent();
		}

		void Description_Populating(object sender, PopulatingEventArgs e)
		{
			var result = new List<string>();
			foreach (var device in FiresecManager.Devices)
			{
				result.Add(device.Description);
			}
			AutoCompleteBox autoCompleteBox = sender as AutoCompleteBox;
			if (autoCompleteBox != null)
			{
				autoCompleteBox.ItemsSource = result;
			}
		}
	}
}