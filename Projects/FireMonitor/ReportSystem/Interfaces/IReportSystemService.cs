using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ReportSystem.DataSets;

namespace ReportSystem.Interfaces
{
	public interface IReportSystemService
	{
		PassCardTemplateLocalizeDataSource GetDataSource();
	}
}
