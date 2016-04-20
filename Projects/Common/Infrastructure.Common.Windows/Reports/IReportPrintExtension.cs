using System.Printing;
using System.Windows;

namespace Infrastructure.Common.Windows.Reports
{
	public interface IReportPrintExtension
	{
		void PreparePrinting(PrintTicket printTicket, Size pageSize);
	}
}
