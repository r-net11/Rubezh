using System;
using System.Data;
using System.Linq;
using FiresecService.Report.DataSources;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class ReflectionReport : BaseReport
	{
		public override DataSet CreateDataSet(DataProvider dataProvider, SKDReportFilter filter)
		{
			var ds = new ReflectioDataSet();
			var mirror = filter as ReflectionReportFilter;
			if (mirror != null && mirror.Mirror != Guid.Empty)
			{
				foreach (var mirrorItem in GKManager.Devices.Where(x => x.Parent != null && x.Parent.UID == mirror.Mirror))
				{
					if (mirrorItem.GKReflectionItem.Zones.Count > 0)
					{
						mirrorItem.GKReflectionItem.Zones.ForEach(x =>
						{
							AddRow(x, ds, mirrorItem.Address);
						});
					}

					if (mirrorItem.GKReflectionItem.Delays.Count > 0)
					{
						mirrorItem.GKReflectionItem.Delays.ForEach(x =>
						{
							AddRow(x, ds, mirrorItem.Address);
						});
					}

					if (mirrorItem.GKReflectionItem.Devices.Count > 0)
					{
						mirrorItem.GKReflectionItem.Devices.ForEach(x =>
						{
							AddRow(x, ds, mirrorItem.Address);
						});
					}

					if (mirrorItem.GKReflectionItem.Diretions.Count > 0)
					{
						mirrorItem.GKReflectionItem.Diretions.ForEach(x =>
						{
							AddRow(x, ds, mirrorItem.Address);
						});
					}

					if (mirrorItem.GKReflectionItem.GuardZones.Count > 0)
					{
						mirrorItem.GKReflectionItem.GuardZones.ForEach(x =>
						{
							AddRow(x, ds, mirrorItem.Address);
						});
					}

					if (mirrorItem.GKReflectionItem.MPTs.Count > 0)
					{
						mirrorItem.GKReflectionItem.MPTs.ForEach(x =>
						{
							AddRow(x, ds, mirrorItem.Address);
						});
					}

					if (mirrorItem.GKReflectionItem.NSs.Count > 0)
					{
						mirrorItem.GKReflectionItem.NSs.ForEach(x =>
						{
							AddRow(x, ds, mirrorItem.Address);
						});
					}
				}
			}
			return ds;
		}

		private void AddRow(GKBase obj, ReflectioDataSet ds, string number)
		{
			var row = ds.Data.NewDataRow();
			row.NO = number;
			row._object = obj.PresentationName;
			ds.Data.AddDataRow(row);
		}
	}
}
