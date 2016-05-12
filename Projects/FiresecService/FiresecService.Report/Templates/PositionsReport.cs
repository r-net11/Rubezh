using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using RubezhDAL;
using FiresecService.Report.Model;

namespace FiresecService.Report.Templates
{
	public partial class PositionsReport : BaseReport
	{
		public PositionsReport()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Портретная ориентация листа согласно требованиям http://172.16.6.113:26000/pages/viewpage.action?pageId=6948166
		/// </summary>
		protected override bool ForcedLandscape
		{
			get { return false; }
		}

		public override string ReportTitle
		{
			get { return "Список должностей организации"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<PositionsReportFilter>();
            var databaseService = new RubezhDAL.DataClasses.DbService();
			dataProvider.LoadCache();
			Guid organisationUID = Guid.Empty;
			var organisations = dataProvider.Organisations.Where(org => filter.User == null || filter.User.IsAdm || org.Value.Item.UserUIDs.Any(y => y == filter.User.UID));
			if (!filter.UseArchive)
				organisations = organisations.Where(org => !org.Value.IsDeleted);
			if (filter.Organisations.IsEmpty())
			{
				if (filter.IsDefault)
					organisationUID = organisations.FirstOrDefault().Key;
			}
			else
			{
				organisationUID = organisations.FirstOrDefault(org => org.Key == filter.Organisations.FirstOrDefault()).Key;
			}
			filter.Organisations = new List<Guid>() { organisationUID };

			var positionFilter = new PositionFilter()
			{
				OrganisationUIDs = filter.Organisations ?? new List<Guid>(),
				UIDs = filter.Positions ?? new List<Guid>(),
				LogicalDeletationType = filter.UseArchive ? LogicalDeletationType.All : LogicalDeletationType.Active,

			};
			var positions = GetPosition(dataProvider, filter);
			var ds = new PositionsDataSet();
			if (positions != null)
				positions.ForEach(position =>
				{
					var row = ds.Data.NewDataRow();
					row.Organisation = position.Organisation;
					row.Position = position.Item.Name;
					row.Description = position.Item.Description;
					ds.Data.AddDataRow(row);
				});
			return ds;
		}
		private static IEnumerable<OrganisationBaseObjectInfo<Position>> GetPosition(DataProvider dataProvider, PositionsReportFilter filter)
		{
			var organisationUID = Guid.Empty;
			var organisations = dataProvider.Organisations.Where(org => filter.User == null || filter.User.IsAdm || org.Value.Item.UserUIDs.Any(y => y == filter.User.UID));
			if (filter.Organisations.IsEmpty())
			{
				if (filter.IsDefault)
					organisationUID = organisations.FirstOrDefault().Key;
			}
			else
			{
				organisationUID = organisations.FirstOrDefault(org => org.Key == filter.Organisations.FirstOrDefault()).Key;
			}

			IEnumerable<OrganisationBaseObjectInfo<Position>> positions = null;
			if (organisationUID != Guid.Empty)
			{
				positions = dataProvider.Positions.Values.Where(item => item.OrganisationUID == organisationUID);

				if (!filter.UseArchive)
					positions = positions.Where(item => !item.IsDeleted);
				if (!filter.Positions.IsEmpty())
					positions = positions.Where(item => filter.Positions.Contains(item.UID));
			}
			return positions != null ? positions : new List<OrganisationBaseObjectInfo<Position>>();
		}
	}
}