using System.Collections.Generic;
using System.Data;
using System.Linq;
using FiresecService.Report.DataSources;
using RubezhAPI;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class DocumentsReport : BaseReport<List<DocumentData>>
	{
		public override List<DocumentData> CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<DocumentsReportFilter>(f);
			var employees = dataProvider.GetEmployees(filter);

			var result = new List<DocumentData>();
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
									var data = new DocumentData();
									data.Organisation = employee.Organisation;
									data.Employee = employee.Name;
									data.Department = employee.Department;
									data.StartDateTime = document.StartDateTime;
									data.EndDateTime = document.EndDateTime;
									data.DocumentCode = documentType.Code;
									data.DocumentName = documentType.Name;
									data.DocumentShortName = documentType.ShortName;
									data.DocumentType = documentType.DocumentType.ToDescription();
									result.Add(data);
								}
							}
						}
					}
				}
			}
			return result;
		}
	}
}
