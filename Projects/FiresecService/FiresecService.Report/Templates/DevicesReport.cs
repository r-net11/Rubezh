using System.Data;
using RubezhAPI;
using FiresecService.Report.DataSources;
using RubezhAPI.GK;
using RubezhAPI.SKD.ReportFilters;
using System.Linq;
using System.Collections.Generic;

namespace FiresecService.Report.Templates
{
	public partial class DevicesReport : BaseReport
	{
		public DevicesReport()
		{
			InitializeComponent();
		}
		protected override bool IsNotDataBase
		{
			get { return true; }
		}
		public override string ReportTitle
		{
			get { return "Список устройств"; }
		}
		public override void ApplyFilter(SKDReportFilter filter)
		{
			base.ApplyFilter(filter);
		}
		protected override DataSet CreateDataSet()
		{
			var dataSet = new DevicesDataSet();
			var devices = GKManager.Devices.Where(device => device.Parent != null && 
			(device.DriverType == GKDriverType.GK
			|| device.DriverType == GKDriverType.RSR2_KAU
			|| (!device.Parent.Driver.IsGroupDevice && device.AllParents.Exists(parent=>parent.DriverType == GKDriverType.RSR2_KAU_Shleif)))
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