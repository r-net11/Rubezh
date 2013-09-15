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

namespace DiagnosticsModule.Views
{
	/// <summary>
	/// Interaction logic for ReportsView.xaml
	/// </summary>
	public partial class ReportsView : UserControl
	{
		public static readonly DependencyProperty ReportSourceProperty = DependencyProperty.Register("ReportSource", typeof(string), typeof(ReportsView), new UIPropertyMetadata(null, OnReportSourceChanged));
		public string ReportSource
		{
			get { return (string)GetValue(ReportSourceProperty); }
			set { SetValue(ReportSourceProperty, value); }
		}
		private static void OnReportSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var reportView = d as ReportsView;
			if (e.Property == ReportSourceProperty)
			{
				var reportSource = e.NewValue as string;
				if (reportSource != null)
					reportView.container.LoadFile(reportSource);
			}
		}

		public ReportsView()
		{
			InitializeComponent();
			SetBinding(ReportSourceProperty, new Binding("PdfPath"));
		}
	}
}
