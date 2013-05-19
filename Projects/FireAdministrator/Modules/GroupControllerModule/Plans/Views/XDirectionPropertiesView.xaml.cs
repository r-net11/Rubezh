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

namespace GKModule.Plans.Views
{
	public partial class XDirectionPropertiesView : UserControl
	{
		public XDirectionPropertiesView()
		{
			InitializeComponent();
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var listView = sender as ListView;
			if (listView != null && listView.SelectedItem != null)
			{
				listView.ScrollIntoView(listView.SelectedItem);
			}
		}
	}
}
