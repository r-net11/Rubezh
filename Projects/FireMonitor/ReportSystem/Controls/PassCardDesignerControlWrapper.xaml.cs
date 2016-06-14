using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.XtraReports.UI;

namespace ReportSystem.Controls
{
	/// <summary>
	/// Interaction logic for PassCardDesignerControlWrapper2.xaml
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
		}

		private static void Report_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var designer = sender as PassCardDesignerControlWrapper;
			if (designer == null) return;

			designer.PassCardDesigner.CurrentReport = (XtraReport) e.NewValue;
			designer.PassCardDesigner.OpenCurrentReport();
		}
	}
}
