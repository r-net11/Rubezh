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
	public class PassCardTemplateReportService
	{
		private PassCardTemplateDataSource _source;
		private PassCardTemplate _passCardTemplate;
		private readonly Guid _organisationId;
		private readonly Guid _passCardTemplateId;

		public PassCardTemplateReportService(ShortPassCardTemplate passCardTemplate, SKDModelBase organisation)
		{
			_source = new PassCardTemplateDataSource();
			_passCardTemplateId = passCardTemplate.UID;
			_organisationId = organisation.UID;
		}

		public Task<PassCardTemplate> GetPassCardTemplate()
		{
			return Task.Factory.StartNew(() => PassCardTemplateHelper.GetDetails(_passCardTemplateId));
		}

		public Task<PassCardTemplateDataSource> GetPassCardTemplateSource()
		{
			return Task.Factory.StartNew(() => AdditionalColumnTypeHelper.GetByOrganisation(_organisationId))
				.ContinueWith(t => AddAdditionalColumns(t.Result.Select(x => x.ToDataColumn())));
		}

		private PassCardTemplateDataSource AddAdditionalColumns(IEnumerable<DataColumn> columns)
		{
			_source.Employee.Columns.AddRange(columns.ToArray());
			return _source;
		}
	}
}
