using System.Windows.Controls;
using System.Windows.Media;

namespace GKModule.Views
{
	public partial class ClausesView : UserControl
	{
		public ClausesView()
		{
			InitializeComponent();
		}

		private void Border_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			stackPanel.Background = Brushes.Gray;
		}

		private void Border_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			stackPanel.Background = Brushes.Transparent;
		}
	}
}