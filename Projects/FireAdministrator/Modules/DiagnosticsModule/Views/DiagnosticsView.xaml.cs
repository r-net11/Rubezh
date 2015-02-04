namespace DiagnosticsModule.Views
{
	public partial class DiagnosticsView
	{
		public DiagnosticsView()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			_mediaElement.Play();
		}
	}
}