using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiresecClient.SKDHelpers;
using StrazhAPI.Extensions;
using StrazhAPI.SKD;
using DataColumn = System.Data.DataColumn;

namespace ReportSystem.DataSets
{
	/// <summary>
	/// Сервис должен разбирать DTO объект и возвращать коллекцию для биндинга к отчёту.
	/// </summary>
	public class PassCardTemplateReportService
	{
	//	private readonly PassCardTemplateLocalizeDataSource _source;
		//private PassCardTemplate _passCardTemplate;
		//private readonly Guid _organisationId;

		public PassCardTemplateReportService()
		{
	//		_source = new PassCardTemplateLocalizeDataSource();
		}

		public PassCardTemplateLocalizeDataSource PassCardTemplateDataSource { get; private set; }


		//public Task<PassCardTemplateLocalizeDataSource> GetPassCardTemplateSource()
		//{
		//	return Task.Factory.StartNew(() => AdditionalColumnTypeHelper.GetByOrganisation(_organisationId))
		//		.ContinueWith(t => AddAdditionalColumns(t.Result.Select(x => x.ToDataColumn())));
		//}

		private static void AddAdditionalColumnsToSource(IEnumerable<DataColumn> columns, PassCardTemplateDataSource source)
		{
			source.Tables[0].Columns.AddRange(columns.ToArray()); //replace Employee by Tables[0]
		}

		public PassCardTemplateLocalizeDataSource GetEmptyDataSource(IEnumerable<DataColumn> columns = null)
		{
			var source = new PassCardTemplateLocalizeDataSource();

			if (columns != null)
				AddAdditionalColumnsToSource(columns, source);

			return source;
		}
	}
}
