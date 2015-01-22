using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;
using System.Linq;
using SKDDriver;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using System.Collections.Generic;

namespace FiresecService.Report.Templates
{
	public partial class Report431 : BaseSKDReport
	{
		public Report431()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Список точек доступа"; }
		}
		protected override DataSet CreateDataSet()
		{
			var filter = GetFilter<ReportFilter431>();
			var dataSet = new DataSet431();

			foreach (var door in SKDManager.Doors)
			{
				if (filter.Doors != null && filter.Doors.Count > 0)
				{
					if (!filter.Doors.Contains(door.UID))
						continue;
				}

				if (filter.Zones != null && filter.Zones.Count > 0)
				{
					var hasZone = false;
					if (door.InDevice != null && filter.Zones.Contains(door.InDevice.ZoneUID))
						hasZone = true;
					if (door.OutDevice != null && filter.Zones.Contains(door.OutDevice.ZoneUID))
						hasZone = true;
					if (!hasZone)
						continue;
				}

				var databaseService = new SKDDatabaseService();

				if (filter.Organisations != null && filter.Organisations.Count > 0)
				{
					var doorUIDs = new List<Guid>();
					foreach (var organisationUID in filter.Organisations)
					{
						var organisationResult = databaseService.OrganisationTranslator.GetSingle(organisationUID);
						if (organisationResult.Result != null)
						{
							doorUIDs.AddRange(organisationResult.Result.DoorUIDs);
						}
					}
					if (!doorUIDs.Contains(door.UID))
						continue;
				}

				var organisationsResult = databaseService.OrganisationTranslator.Get(new OrganisationFilter());
				if (organisationsResult.Result != null)
				{
					foreach (var organisation in organisationsResult.Result)
					{
						if (organisation.DoorUIDs.Contains(door.UID))
						{
							var dataRow = dataSet.Data.NewDataRow();
							dataRow.Number = door.No;
							dataRow.Door = door.Name;
							dataRow.Comment = door.Description;
							if (door.InDevice != null)
							{
								dataRow.EnterReader = door.InDevice.Name;
								if (door.InDevice.Zone != null)
									dataRow.ExitZone = door.InDevice.Zone.Name;
								if (door.InDevice.Parent != null)
								{
									dataRow.Controller = door.InDevice.Parent.Name;
									dataRow.IP = door.InDevice.Parent.Address;
								}
							}
							if (door.OutDevice != null)
							{
								dataRow.ExitReader = door.OutDevice.Name;
								if (door.OutDevice.Zone != null)
									dataRow.EnterZone = door.OutDevice.Zone.Name;
							}

							dataRow.Organisation = organisation.Name;

							dataSet.Data.Rows.Add(dataRow);
						}
					}
				}
			}

			return dataSet;
		}
		//protected override void DataSourceRequered()
		//{
		//    base.DataSourceRequered();
		//    FillTestData();
		//}
	}
}