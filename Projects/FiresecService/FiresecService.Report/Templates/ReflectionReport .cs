using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using RubezhDAL;
using RubezhAPI;
using RubezhAPI.GK;

namespace FiresecService.Report.Templates
{
	public partial class ReflectionReport : BaseReport
	{
		public ReflectionReport()
		{
			InitializeComponent();
		}

		protected override bool IsNotDataBase
		{
			get { return true; }
		}

		public override string ReportTitle
		{
			get
			{
				var mirror = Filter as ReflectionReportFilter;
				if (mirror != null && mirror.Mirror != Guid.Empty)
				{
					var device = GKManager.Devices.FirstOrDefault(x => x.UID == mirror.Mirror);
					if(device != null)
						return "Список отражений для " + device.PresentationName;
				}
				return "Список отражений";
			}
		}

		public override void ApplyFilter(SKDReportFilter filter)
		{
			base.ApplyFilter(filter);
		}

		protected override DataSet CreateDataSet()
		{
			var ds = new ReflectioDataSet();
			var mirror = Filter as ReflectionReportFilter;
			if (mirror != null && mirror.Mirror != Guid.Empty)
			{
				foreach (var mirrorItem in GKManager.Devices.Where(x => x.Parent != null && x.Parent.UID == mirror.Mirror))
				{
					if (mirrorItem.GKMirrorItem.Zones.Count > 0)
					{
						mirrorItem.GKMirrorItem.Zones.ForEach(x =>
						{
							AddRow(x, ds, mirrorItem.Address);
						});	
					}

					if (mirrorItem.GKMirrorItem.Delays.Count > 0)
					{
						mirrorItem.GKMirrorItem.Delays.ForEach(x =>
						{
							AddRow(x, ds, mirrorItem.Address);
						});
					}

					if (mirrorItem.GKMirrorItem.Devices.Count > 0)
					{
						mirrorItem.GKMirrorItem.Devices.ForEach(x =>
						{
							AddRow(x, ds, mirrorItem.Address);
						});
					}

					if (mirrorItem.GKMirrorItem.Diretions.Count > 0)
					{
						mirrorItem.GKMirrorItem.Diretions.ForEach(x =>
						{
							AddRow(x, ds, mirrorItem.Address);
						});
					}

					if (mirrorItem.GKMirrorItem.GuardZones.Count > 0)
					{
						mirrorItem.GKMirrorItem.GuardZones.ForEach(x =>
						{
							AddRow(x, ds, mirrorItem.Address);
						});
					}

					if (mirrorItem.GKMirrorItem.MPTs.Count > 0)
					{
						mirrorItem.GKMirrorItem.MPTs.ForEach(x =>
						{
							AddRow(x, ds, mirrorItem.Address);
						});
					}

					if (mirrorItem.GKMirrorItem.NSs.Count > 0)
					{
						mirrorItem.GKMirrorItem.NSs.ForEach(x =>
						{
							AddRow(x, ds, mirrorItem.Address);
						});
					}
				}
			}
			return ds;
		}

		private void AddRow(GKBase obj, ReflectioDataSet ds,string number)
		{
			var row = ds.Data.NewDataRow();
			row.NO = number;
			row._object = obj.PresentationName;
			ds.Data.AddDataRow(row);			
		}
	}
}