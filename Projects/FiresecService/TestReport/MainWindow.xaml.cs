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
using FiresecService.Report;
using DevExpress.Xpf.Printing;
using System.Reflection;
using System.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.Client;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.Templates;

namespace TestReport
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			WindowState = System.Windows.WindowState.Maximized;
			Button_Click(this, new RoutedEventArgs());
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var report = new Report418();
			report.ApplyFilter(new ReportFilter418());
			var model = new XtraReportPreviewModel(report)
			{
				IsParametersPanelVisible = false,
				AutoShowParametersPanel = false,
				IsDocumentMapVisible = false,
				ZoomMode = new ZoomFitModeItem(ZoomFitMode.WholePage),
			};
			documentViewer1.Model = model;
			report.CreateDocument();
		}

		private void ServerButton_Click(object sender, RoutedEventArgs e)
		{
			ReportServiceManager.Run();
			var model = CreateServiceModel();
			documentViewer1.Model = model;
			model.ReportName = "Report418";
			var args = new ReportFilter418();
			var method = typeof(ReportServicePreviewModel).GetMethod("CreateDocument", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(object) }, null);
			method.Invoke(model, new object[] { args });
		}
		private ReportServicePreviewModel CreateServiceModel()
		{
			var url = "net.pipe://127.0.0.1/ReportFiresecService/";
			var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			binding.OpenTimeout = TimeSpan.FromMinutes(10);
			binding.SendTimeout = TimeSpan.FromMinutes(10);
			binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
			binding.MaxReceivedMessageSize = Int32.MaxValue;
			binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
			binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
			binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;
			binding.Security.Mode = NetNamedPipeSecurityMode.None;

			var endpoint = new EndpointAddress(url);
			var factory = new ReportServiceClientFactory(endpoint, binding);
			return new ReportServicePreviewModel()
			{
				ServiceClientFactory = factory,
				IsParametersPanelVisible = false,
				AutoShowParametersPanel = false,
				IsDocumentMapVisible = false,
				ZoomMode = new ZoomFitModeItem(ZoomFitMode.WholePage),
			};
		}
	}
}
