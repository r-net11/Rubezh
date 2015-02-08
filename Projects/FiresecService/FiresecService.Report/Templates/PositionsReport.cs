using System;
using System.Linq;
using Common;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;
using FiresecAPI.SKD.ReportFilters;
using System.Text;
using System.Collections.Generic;
using FiresecService.Report.DataSources;
using SKDDriver;
using FiresecAPI.SKD;

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
				filter.Organisations = new List<Guid>() { dataProvider.Organisations.First(org => !org.Value.IsDeleted).Value.UID };

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