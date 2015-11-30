using RubezhAPI.GK;
using System.Collections.Generic;

namespace RubezhAPI.SKD.ReportFilters
{
	public class DevicesReportFilter : SKDReportFilter
	{
		public List<GKDevice> SelectedDevices { get; set; }
		public DevicesReportFilter()
		{
			SelectedDevices = new List<GKDevice>();
		}
	}
}