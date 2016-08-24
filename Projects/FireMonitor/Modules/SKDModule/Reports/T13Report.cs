using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CodeReason.Reports;
using Localization.SKD.Common;
using StrazhAPI.SKD;
using Infrastructure.Common.Reports;
using Common;

namespace SKDModule.Reports
{
	internal class T13Report : ISingleReportProvider //IMultiReportProvider
	{
		public ReportT13 ReportModel { get; private set; }

		public T13Report(ReportT13 reportModel)
		{
			ReportModel = reportModel;
		}

		#region ISingleReportProvider Members

		ReportData ISingleReportProvider.GetData()
		{
			var table = BuildTable();
			ReportData data = CreateData();
			data.DataTables.Add(table);
			return data;
		}

		#endregion

		#region IMultiReportProvider Members

		//IEnumerable<ReportData> IMultiReportProvider.GetData()
		//{
		//	var result = new List<ReportData>();
		//	var table = BuildTable();
		//	ReportData data = null;
		//	int countOnPage = -1;
		//	for (int i = 0; i < table.Rows.Count; i++)
		//	{
		//		if (data == null || data.DataTables[0].Rows.Count == countOnPage)
		//		{
		//			data = CreateData();
		//			data.DataTables.Add(table.Clone());
		//			result.Add(data);
		//			if (countOnPage == -1)
		//			{
		//				data.Groups.Add("Header");
		//				countOnPage = 2;
		//			}
		//			else if (i + 3 == table.Rows.Count)
		//				countOnPage = 2;
		//			else if (i + 3 <= table.Rows.Count)
		//				countOnPage = 3;
		//			if (i + 3 > table.Rows.Count)
		//				data.Groups.Add("Footer");
		//		}
		//		data.DataTables[0].Rows.Add(table.Rows[i].ItemArray);
		//	}
		//	return result;
		//}

		#endregion

		#region IReportProvider Members

		public string Template
		{
			get { return "Reports/T13_2.xaml"; }
		}

		public string Title
		{
			get { return string.Format(CommonResources.T13, ReportModel.StartDateTime, ReportModel.EndDateTime); }
		}

		public bool IsEnabled
		{
			get { return true; }
		}

		public IReportPdfProvider PdfProvider
		{
			get { return null; }
		}

		#endregion

		private ReportData CreateData()
		{
			var data = new ReportData();
			data.ReportDocumentValues.Add("PrintDate", ReportModel.CreationDateTime);
			data.ReportDocumentValues.Add("StartDate", ReportModel.StartDateTime);
			data.ReportDocumentValues.Add("EndDate", ReportModel.EndDateTime);

			data.ReportDocumentValues.Add("FillName", ReportModel.FillName);
			data.ReportDocumentValues.Add("HRName", ReportModel.HRName);
			data.ReportDocumentValues.Add("LeadName", ReportModel.LeadName);

			data.ReportDocumentValues.Add("FillPosition", ReportModel.FillPosition);
			data.ReportDocumentValues.Add("HRPosition", ReportModel.HRPosition);
			data.ReportDocumentValues.Add("LeadPosition", ReportModel.LeadPosition);

			data.ReportDocumentValues.Add("Organization", ReportModel.OrganisationName);
			data.ReportDocumentValues.Add("Department", ReportModel.DepartmentName);

			return data;
		}
		private DataTable BuildTable()
		{
			var table = new DataTable("Data");
			table.Columns.AddRange(GetColumns().Select(name => new DataColumn(name, typeof(string))).ToArray());
			foreach (var employee in ReportModel.EmployeeRepors)
			{
				var values = new List<string>
				{
					employee.No.ToString(),
					employee.EmployeeFIO,
					employee.TabelNo,
					GetValue(employee.FirstHalfDaysCount),
					GetValue(employee.SecondHalfDaysCount),
					GetValue(employee.TotalDaysCount),
					GetValue(employee.FirstHalfTimeSpan),
					GetValue(employee.SecondHalfTimeSpan),
					GetValue(employee.TotalTimeSpan),
				};
				for (int i = 0; i < 31; i++)
					values.AddRange(new string[] { null, null });
				for (int i = 0; i < employee.Days.Count; i++)
					if (ReportModel.StartDateTime.AddDays(i).Date <= ReportModel.EndDateTime.Date)
					{
						var num = ReportModel.StartDateTime.AddDays(i).Day;
						values[7 + 2 * num] = employee.Days[i].CodeStrings;
						values[8 + 2 * num] = employee.Days[i].TimeSpanStrings;
					}
				for (int i = 0; i < 8; i++)
					values.AddRange(
						employee.MissReasons.Count > i ?
						new string[] { employee.MissReasons[i].Code, GetValue(employee.MissReasons[i].TimeSpan) } :
						new string[] { null, null });
				var newRow = table.NewRow();
				newRow.ItemArray = values.ToArray();
				table.Rows.Add(newRow);
			}
			return table;
		}
		private IEnumerable<string> GetColumns()
		{
			yield return "No";
			yield return "EmploueeFIO";
			yield return "TabelNo";
			yield return "FirstHalfDays";
			yield return "SecondHalfDays";
			yield return "TotalDays";
			yield return "FirstHalfHours";
			yield return "SecondHalfHours";
			yield return "TotalHours";
			for (int i = 1; i <= 31; i++)
			{
				yield return string.Format("Day{0}_Code", i);
				yield return string.Format("Day{0}_Hours", i);
			}
			for (int i = 1; i <= 8; i++)
			{
				yield return string.Format("MissReason{0}_Code", i);
				yield return string.Format("MissReason{0}_Hours", i);
			}
		}

		private string GetValue(int val)
		{
			return val > 0 ? val.ToString() : null;
		}
		private string GetValue(TimeSpan val)
		{
			return val != TimeSpan.Zero ? val.TotalHours.ToString("n2").TrimEnd('0').TrimEnd(',') : null;
		}
	}
}