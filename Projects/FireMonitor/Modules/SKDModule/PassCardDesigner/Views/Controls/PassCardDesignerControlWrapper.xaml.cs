using DevExpress.XtraReports.UI;
using System.Windows;
using System.Windows.Controls;

namespace SKDModule.PassCardDesigner.Views
{
	/// <summary>
	/// Interaction logic for PassCardDesignerControlWrapper.xaml
	/// </summary>
	public partial class PassCardDesignerControlWrapper : UserControl
	{
		public static readonly DependencyProperty ReportProperty = DependencyProperty.Register("Report", typeof(XtraReport),
			typeof(PassCardDesignerControlWrapper), new FrameworkPropertyMetadata((Report_Changed)));


		public XtraReport Report
		{
			get { return (XtraReport)GetValue(ReportProperty); }
			set { SetValue(ReportProperty, value); }
		}

		public PassCardDesignerControlWrapper()
		{
			InitializeComponent();
			Loaded += (s, e) =>
			{
				var currentWindow = Window.GetWindow(this); // get the parent window
				if (currentWindow == null) return;
				currentWindow.Closing += (s1, e1) => Dispose();
			};
		}

		private void Dispose()
		{
			if (PassCardDesigner != null)
				PassCardDesigner.Dispose();
			if(WindowsFormsHost != null)
				WindowsFormsHost.Dispose();
		}

		private static void Report_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var designer = sender as PassCardDesignerControlWrapper;
			if (designer == null) return;

			designer.PassCardDesigner.CurrentReport = (XtraReport)e.NewValue;
			designer.PassCardDesigner.OpenCurrentReport();
		}
	}
}
