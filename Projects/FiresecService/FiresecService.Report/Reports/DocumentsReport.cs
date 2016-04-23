using System.Data;
using System.Linq;
using FiresecService.Report.DataSources;
using RubezhAPI;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class DocumentsReport : BaseReport
	{
		public override DataSet CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<DocumentsReportFilter>(f);
			var employees = dataProvider.GetEmployees(filter);
			var ds = new DataSetDocuments();
			foreach (var employee in employees)
			{
				var documentsResult = dataProvider.DbService.TimeTrackDocumentTranslator.Get(employee.UID, filter.DateTimeFrom, filter.DateTimeTo);
				if (documentsResult.Result != null)
				{
					foreach (var document in documentsResult.Result)
					{
						var documentTypesResult = dataProvider.DbService.TimeTrackDocumentTypeTranslator.Get(employee.OrganisationUID);
						if (documentTypesResult.Result != null)
						{
							var documentType = documentTypesResult.Result.FirstOrDefault(x => x.Code == document.DocumentCode);
							if (documentType == null)
								documentType = TimeTrackDocumentTypesCollection.TimeTrackDocumentTypes.FirstOrDefault(x => x.Code == document.DocumentCode);
							if (documentType != null)
							{
								if (filter.Abcense && documentType.DocumentType == DocumentType.Absence ||
								   filter.Presence && documentType.DocumentType == DocumentType.Presence ||
									filter.Overtime && documentType.DocumentType == DocumentType.Overtime)
								{
									var row = ds.Data.NewDataRow();
									row.Organisation = employee.Organisation;
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
					}
				}
			}
			return ds;
		}
	}
}
