using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.XtraReports.Service;
using DevExpress.XtraReports.UI;

namespace FiresecService.Report
{
	internal class FiresecReportService : ReportService
	{
		public override DocumentId StartBuild(InstanceIdentity instanceIdentity, ReportBuildArgs buildArgs)
		{
			return base.StartBuild(instanceIdentity, buildArgs);
		}

		protected override void FillDataSources(XtraReport report, string reportName, bool isDesignActive)
		{
			base.FillDataSources(report, reportName, isDesignActive);
		}

		public override void StopBuild(DocumentId documentId)
		{
			base.StopBuild(documentId);
		}

		public override ExportId StartExport(DocumentId documentId, DocumentExportArgs exportArgs)
		{
			return base.StartExport(documentId, exportArgs);
		}
	}
}