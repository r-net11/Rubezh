using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Windows;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.Xpf.Printing;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report;
using FiresecService.Report.Templates;
using System.Collections.Generic;
using System.Security.Principal;
using FiresecService;
using SKDDriver.Translators;
using FiresecAPI.SKD;
using EmployeeReport = FiresecService.Report.Templates.EmployeeReport;

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
			ConfigurationCashHelper.Update();
			PassJournalTranslator.ConnectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=PassJournal_1;Integrated Security=True;Language='English'";
			var report = new WorkingTimeReport();
			var filter = new WorkingTimeReportFilter()
			{
				//SortColumn = "Number",
				//PassCardForcing = false,
				//PassCardLocked = false,
				//UseArchive = true,
				//Organisations = new List<Guid>() { new Guid("F6E5DA71-C4D7-4421-94A9-F5F7ED7DDF7E") },
				//PeriodType = ReportPeriodType.Month,
				//DateTimeFrom = new DateTime(2015,01,01),
				//DateTimeTo = DateTime.Today.AddDays(1),
			};
			filter.Timestamp = DateTime.Now;
			filter.User = WindowsIdentity.GetCurrent().Name;
			report.ApplyFilter(filter);
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
			model.ReportName = "WorkingTimeReport";
			var args = new WorkingTimeReportFilter();
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
