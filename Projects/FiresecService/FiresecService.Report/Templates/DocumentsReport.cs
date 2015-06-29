using System;
using FiresecAPI;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using SKDDriver;

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
			get { return "Отчет по оправдательным документам"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<DocumentsReportFilter>();
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
