using System.Windows;

namespace Diagnostics
{
	public partial class DiagnosticsView : Window
	{
		public DiagnosticsView()
		{
			InitializeComponent();
			var diagnosticsViewModel = new DiagnosticsViewModel();
			DataContext = diagnosticsViewModel;
		}
	}
}