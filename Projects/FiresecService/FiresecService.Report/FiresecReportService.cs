using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraReports.Service;
using DevExpress.DocumentServices.ServiceModel.DataContracts;

namespace FiresecService.Report
{
	class FiresecReportService : ReportService
	{
		public override DocumentId StartBuild(InstanceIdentity instanceIdentity, ReportBuildArgs buildArgs)
		{
			return base.StartBuild(instanceIdentity, buildArgs);
		}
	}
}
