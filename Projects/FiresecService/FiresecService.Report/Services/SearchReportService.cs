using FiresecService.Report.Operations;
using ReportSystem.Api.DTO;
using StrazhAPI.Printing;
using StrazhAPI.SKD;
using StrazhDAL;
using System;
using System.Collections.Generic;

namespace FiresecService.Report.Services
{
	public sealed class SearchReportService
	{
		private readonly SKDDatabaseService _dataService;
		private readonly EmployeeFilter _filter;
		private readonly Guid? _selectedTemplateId;
		private BaseReportSearchOperation _searchOperation;
		private IEnumerable<Employee> _employees;

		public SearchReportService(EmployeeFilter filter, Guid? selectedTemplateId, SKDDatabaseService dataService)
		{
			if (filter == null)
				throw new ArgumentNullException("filter");

			_filter = filter;
			_selectedTemplateId = selectedTemplateId;
			_dataService = dataService;
		}

		public IEnumerable<ReportDTO> SearchResult { get; private set; }

		public void Execute()
		{
			_employees = _dataService.EmployeeTranslator.GetFullList(_filter).Result;

			if (_selectedTemplateId.HasValue)
			{
				_searchOperation = new GeneralReportSearchOperation(_dataService, _selectedTemplateId.Value, _employees);
			}
			else
				_searchOperation = new AppropriateReportSearchOperation(_dataService, _employees);

			_searchOperation.Execute();
			SearchResult = _searchOperation.Result;
		}
	}
}
