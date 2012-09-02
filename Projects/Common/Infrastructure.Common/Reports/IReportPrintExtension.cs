using System.Collections.Generic;
using System.Printing;
using System.Windows;
using CodeReason.Reports;
using FiresecAPI.Models;

namespace Infrastructure.Common.Reports
{
	public interface IReportPrintExtension
	{
		void PreparePrinting(PrintTicket printTicket, Size pageSize);
	}
}
