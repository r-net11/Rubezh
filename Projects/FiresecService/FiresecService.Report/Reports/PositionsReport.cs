using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;
using FiresecService.Report.Model;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class PositionsReport : BaseReport<List<PositionsData>>
	{
		public override List<PositionsData> CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<PositionsReportFilter>(f);
			var databaseService = new RubezhDAL.DataClasses.DbService();
			dataProvider.LoadCache();
			Guid organisationUID = Guid.Empty;
			var organisations = dataProvider.Organisations.Where(org => org.Value.Item.UserUIDs.Any(y => y == filter.UserUID));
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
			List<PositionsData> result = new List<PositionsData>();
			if (positions != null)
				positions.ForEach(position =>
				{
					PositionsData row = new PositionsData();
					row.Organisation = position.Organisation;
					row.Position = position.Item.Name;
					row.Description = position.Item.Description;
					result.Add(row);
				});
			return result;
		}

		private static IEnumerable<OrganisationBaseObjectInfo<Position>> GetPosition(DataProvider dataProvider, PositionsReportFilter filter)
		{
			var organisationUID = Guid.Empty;
			var organisations = dataProvider.Organisations.Where(org => org.Value.Item.UserUIDs.Any(y => y == filter.UserUID));
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
