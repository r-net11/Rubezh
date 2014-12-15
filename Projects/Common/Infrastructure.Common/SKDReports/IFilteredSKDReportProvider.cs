using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common.SKDReports
{
	public interface IFilteredSKDReportProvider : ISKDReportProvider
	{
		Type FilterType { get; }
		object FilterObject { get; }
		bool ChangeFilter();
	}
}
