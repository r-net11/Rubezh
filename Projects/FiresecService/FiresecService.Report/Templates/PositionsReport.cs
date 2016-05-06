using Common;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using StrazhDAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
			dataProvider.LoadCache();
			Guid organisationUID;
			var organisations = dataProvider.Organisations.OrderBy(x => x.Value.Name).Where(org => org.Value.Item.UserUIDs.Any(y => y == filter.UserUID));

			if (!filter.UseArchive)
				organisations = organisations.Where(org => !org.Value.IsDeleted);

			if (filter.Organisations.IsEmpty() && filter.IsDefault)
				organisationUID = organisations.FirstOrDefault().Key;
			else
				organisationUID = organisations.FirstOrDefault(org => org.Key == filter.Organisations.FirstOrDefault()).Key;

			filter.Organisations = new List<Guid> { organisationUID };

			var positionFilter = new PositionFilter
			{
				OrganisationUIDs = filter.Organisations,
				UIDs = filter.Positions ?? new List<Guid>(),
				LogicalDeletationType = filter.UseArchive ? LogicalDeletationType.All : LogicalDeletationType.Active,
			};

			var positions = dataProvider.DatabaseService.PositionTranslator.Get(positionFilter);
			var ds = new PositionsDataSet();
			if (positions.Result != null)
				positions.Result.ForEach(position =>
				{
					var row = ds.Data.NewDataRow();
					row.Position = position.Name;
					row.Description = position.Description;
					ds.Data.AddDataRow(row);
				});

			return ds;
		}
	}
}