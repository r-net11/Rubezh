using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using SKDDriver;
using FiresecClient;
using FiresecAPI.GK;

namespace FiresecService.Report.Templates
{
	public partial class ReflectionReport : BaseReport
	{
		public ReflectionReport()
		{
			InitializeComponent();
		}

		///// <summary>
		/// Портретная ориентация листа согласно требованиям http://172.16.6.113:26000/pages/viewpage.action?pageId=6948166
		/// </summary>

		protected override bool IsNotDateBase
		{
			get { return true; }
		}

		public override string ReportTitle
		{
			get
			{
				var mirror = Filter as ReflectionReportFilter;
				if (mirror != null && mirror.Mirror != Guid.Empty)
				return "Список отражений для " + GKManager.Devices.FirstOrDefault(x => x.UID == mirror.Mirror).PresentationName;
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
				var mirrorItems = GKManager.Devices.Where(x => x.Parent != null && x.Parent.UID == mirror.Mirror);
				
				foreach (var mirrorItem in mirrorItems)
				{
					if (mirrorItem.GKReflectionItem.Zones.Count > 0)
					{
						mirrorItem.GKReflectionItem.Zones.ForEach(x =>
						{
							SetValue(x, ds, mirrorItem.Address);
						});	
					}

					if (mirrorItem.GKReflectionItem.Delays.Count > 0)
					{
						mirrorItem.GKReflectionItem.Delays.ForEach(x =>
						{
							SetValue(x, ds, mirrorItem.Address);
						});
					}

					if (mirrorItem.GKReflectionItem.Devices.Count > 0)
					{
						mirrorItem.GKReflectionItem.Devices.ForEach(x =>
						{
							SetValue(x, ds, mirrorItem.Address);
						});
					}

					if (mirrorItem.GKReflectionItem.Diretions.Count > 0)
					{
						mirrorItem.GKReflectionItem.Diretions.ForEach(x =>
						{
							SetValue(x, ds, mirrorItem.Address);
						});
					}

					if (mirrorItem.GKReflectionItem.GuardZones.Count > 0)
					{
						mirrorItem.GKReflectionItem.GuardZones.ForEach(x =>
						{
							SetValue(x, ds, mirrorItem.Address);
						});
					}

					if (mirrorItem.GKReflectionItem.MPTs.Count > 0)
					{
						mirrorItem.GKReflectionItem.MPTs.ForEach(x =>
						{
							SetValue(x, ds, mirrorItem.Address);
						});
					}

					if (mirrorItem.GKReflectionItem.NSs.Count > 0)
					{
						mirrorItem.GKReflectionItem.NSs.ForEach(x =>
						{
							SetValue(x, ds, mirrorItem.Address);
						});
					}
				}
			}
			return ds;
		}

		private void  SetValue(GKBase obj, ReflectioDataSet ds,string number)
		{
			var row = ds.Data.NewDataRow();
			row.NO = number;
			row._object = obj.PresentationName;
			ds.Data.AddDataRow(row);			
		}
	}
}

