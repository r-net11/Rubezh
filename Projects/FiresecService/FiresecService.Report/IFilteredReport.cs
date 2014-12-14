using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecService.Report
{
	public interface IFilteredReport
	{
		void ApplyFilter(object filter);
	}
}
