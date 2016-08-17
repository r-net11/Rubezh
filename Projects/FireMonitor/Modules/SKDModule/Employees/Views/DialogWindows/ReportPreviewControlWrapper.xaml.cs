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
using ReportSystem.Reports;
using SKDModule.PassCardDesigner.Model;

namespace SKDModule.Employees.Views.DialogWindows
{
	/// <summary>
	/// Interaction logic for ReportPreviewControlWrapper.xaml
	/// </summary>
	public partial class ReportPreviewControlWrapper : UserControl
	{
		public static DependencyProperty CurrentReportProperty = DependencyProperty.Register("CurrentReport",
			typeof (PassCardTemplateReport), typeof (ReportPreviewControlWrapper), new FrameworkPropertyMetadata(OnReportValueChanged));

		private static void OnReportValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var preview = d as ReportPreviewControlWrapper;
			if (preview == null || preview.ReportPreviewControl == null) return;

			preview.ReportPreviewControl.SetReport((PassCardTemplateReport)e.NewValue);
			//designer.PassCardDesigner.CurrentReport = (XtraReport)e.NewValue;
			//designer.PassCardDesigner.OpenCurrentReport();
		}

		public PassCardTemplateReport CurrentReport
		{
			get { return (PassCardTemplateReport) GetValue(CurrentReportProperty); }
			set { SetValue(CurrentReportProperty, value); }
		}

		public ReportPreviewControlWrapper()
		{
			InitializeComponent();
			Loaded += (s, e) =>
			{
				var win = Window.GetWindow(this);
				if (win == null) return;
				win.Closing += (s1, e1) => Dispose();
			};
		}

		private void Dispose()
		{
			if(ReportPreviewControl != null)
				ReportPreviewControl.Dispose();
			if(FormsHost != null)
				FormsHost.Dispose();
		}
	}
}
