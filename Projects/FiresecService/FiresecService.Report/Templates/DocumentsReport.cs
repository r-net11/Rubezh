using FiresecService.Report.DataSources;
using Localization.FiresecService.Report.Common;
using StrazhAPI;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using System.Data;
using System.Linq;

namespace FiresecService.Report.Templates
{
	public partial class DocumentsReport : BaseReport
	{
		public DocumentsReport()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Альбомная ориентация листа согласно требованиям http://172.16.6.113:26000/pages/viewpage.action?pageId=6948166
		/// </summary>
		protected override bool ForcedLandscape
		{
			get { return true; }
		}

		public override string ReportTitle
		{
			get { return CommonResources.DocumentsReport; }
		}

		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<DocumentsReportFilter>();
			var employees = dataProvider.GetEmployees(filter, true);
			var ds = new DataSetDocuments();

			foreach (var employee in employees)
			{
				var documentsResult = dataProvider.DatabaseService.TimeTrackDocumentTranslator.Get(employee.UID, filter.DateTimeFrom, filter.DateTimeTo);

				if (documentsResult.Result == null) continue;

				foreach (var document in documentsResult.Result)
				{
					var documentTypesResult = dataProvider.DatabaseService.TimeTrackDocumentTypeTranslator.Get(employee.OrganisationUID);

					if (documentTypesResult.Result == null) continue;

					var documentType = documentTypesResult.Result.FirstOrDefault(x => x.Code == document.DocumentCode);

					if (documentType == null) continue;

					if (filter.Abcense && documentType.DocumentType == DocumentType.Absence ||
						filter.Presence && documentType.DocumentType == DocumentType.Presence ||
						filter.Overtime && documentType.DocumentType == DocumentType.Overtime ||
						filter.AbcenseReasonable && documentType.DocumentType == DocumentType.AbsenceReasonable)
					{
						var row = ds.Data.NewDataRow();
						row.Employee = employee.Name;
						row.Department = employee.Department;
						row.StartDateTime = document.StartDateTime;
						row.EndDateTime = document.EndDateTime;
						row.DocumentCode = documentType.Code;
						row.DocumentName = documentType.Name;
						row.DocumentShortName = documentType.ShortName;
						row.DocumentType = documentType.DocumentType.ToDescription();
						ds.Data.AddDataRow(row);
					}
				}
			}
			return ds;
		}

		protected override void BeforeReportPrint()
		{
			base.BeforeReportPrint();

			this.xrTableCell10.Text = CommonResources.Employee;
			this.xrTableCell11.Text = CommonResources.StartDate;
			this.xrTableCell12.Text = CommonResources.EndDate;
			this.xrTableCell13.Text = CommonResources.StartTime;
			this.xrTableCell8.Text = CommonResources.EndTime;
			this.xrTableCell1.Text = CommonResources.Type;
			this.xrTableCell9.Text = CommonResources.Document;
			this.xrTableCell2.Text = CommonResources.LitteralCode;
			this.xrTableCell15.Text = CommonResources.NumCode;
		}
	}
}