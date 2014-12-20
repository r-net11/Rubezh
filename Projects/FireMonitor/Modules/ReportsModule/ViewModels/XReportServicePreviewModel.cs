using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Printing;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.ServiceOperations;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using System.Collections.ObjectModel;

namespace ReportsModule.ViewModels
{
	public class XReportServicePreviewModel : ReportServicePreviewModel
	{
		public XReportServicePreviewModel()
			: base()
		{
		}
		public XReportServicePreviewModel(string s)
			: base(s)
		{
		}
		public void Build(object args)
		{
			CreateDocument(args);
		}
		public IReportServiceClient ServiceClient
		{
			get { return Client; }
		}

		protected override CreateDocumentOperation ConstructCreateDocumentOperation(ReportBuildArgs buildArgs)
		{
			var operation = base.ConstructCreateDocumentOperation(buildArgs);
			return operation;
		}

		private ReadOnlyCollection<double> _zoomValues;
		protected override ReadOnlyCollection<double> ZoomValues
		{
			get { return _zoomValues ?? (_zoomValues = new ReadOnlyCollection<double>(new double[] { 10.0, 25.0, 50.0, 75.0, 100.0, 150.0, 200.0, 300.0, 400.0, 500.0 })); }
		}
	}
}
