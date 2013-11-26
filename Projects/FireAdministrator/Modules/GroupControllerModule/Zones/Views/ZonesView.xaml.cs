using System.Windows.Controls;

namespace GKModule.Views
{
	public partial class ZonesView : UserControl
	{
		public ZonesView()
		{
			InitializeComponent();
			if (height != 0)
				bottomRow.Height = new System.Windows.GridLength(height);
			LayoutUpdated += new System.EventHandler(ZonesView_LayoutUpdated);
		}

		void ZonesView_LayoutUpdated(object sender, System.EventArgs e)
		{
			//var width = grid.Columns[0].Width;
			//grid.Columns[0].Width = new DataGridLength(0);
			//grid.Columns[0].Width = width;
		}

		static double height = 0;

		private void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
		{
			height = bottomRow.Height.Value;
		}

		private void XDataGrid_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
		{
			var grid = (DataGrid)sender;
		}
	}
}