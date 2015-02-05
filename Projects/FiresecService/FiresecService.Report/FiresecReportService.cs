using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraReports.Service;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.XtraReports.UI;

namespace FiresecService.Report
{
	class FiresecReportService : ReportService
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
		
	}
}
