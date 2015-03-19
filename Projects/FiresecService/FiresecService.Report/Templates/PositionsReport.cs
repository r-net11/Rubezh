using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using SKDDriver;

namespace FiresecService.Report.Templates
{
	public partial class PositionsReport : BaseReport
	{
		public PositionsReport()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Список должностей организации"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<PositionsReportFilter>();
			var databaseService = new SKDDatabaseService();
			dataProvider.LoadCache();
			if (filter.Organisations.IsEmpty())
				filter.Organisations = new List<Guid>() { dataProvider.Organisations.First(org => !org.Value.IsDeleted && org.Value.Item.UserUIDs.Any(y => y == filter.UserUID)).Value.UID };

			var positionFilter = new PositionFilter()
			{
				OrganisationUIDs = filter.Organisations ?? new List<Guid>(),
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