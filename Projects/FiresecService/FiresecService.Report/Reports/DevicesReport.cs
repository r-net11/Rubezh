using System.Collections.Generic;
using System.Data;
using System.Linq;
using FiresecService.Report.DataSources;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class DevicesReport : BaseReport
	{
		public override DataSet CreateDataSet(DataProvider dataProvider, SKDReportFilter filter)
		{
			var dataSet = new DevicesDataSet();
			var devices = GKManager.Devices.Where(device => device.Parent != null &&
			(device.DriverType == GKDriverType.GK
			|| device.DriverType == GKDriverType.RSR2_KAU
			|| (!device.Parent.Driver.IsGroupDevice && device.AllParents.Exists(parent => parent.DriverType == GKDriverType.RSR2_KAU_Shleif)))
			&& device.DriverType != GKDriverType.KAUIndicator
			&& device.DriverType != GKDriverType.RSR2_MVP_Part);
			var dictionary = new Dictionary<string, int>();
			foreach (var device in devices)
			{
				var key = device.ShortName;
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, 1);
				}
				else
				{
					dictionary[key]++;
				}
			}
			foreach (var item in dictionary)
			{
				var dataRow = dataSet.Data.NewDataRow();
				dataRow.Nomination = item.Key;
				dataRow.Number = item.Value;
				dataSet.Data.Rows.Add(dataRow);
			}
			return dataSet;
		}
	}
}
