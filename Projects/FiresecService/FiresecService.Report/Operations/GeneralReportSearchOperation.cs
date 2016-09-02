using System.Diagnostics;
using ReportSystem.Api.DTO;
using StrazhAPI.SKD;
using StrazhDAL;
using System;
using System.Collections.Generic;

namespace FiresecService.Report.Operations
{
	public sealed class GeneralReportSearchOperation : BaseReportSearchOperation
	{
		private readonly SKDDatabaseService _dataService;
		private readonly Guid? _selectedTemplateId;
		private readonly IEnumerable<Employee> _employees;
		private PassCardTemplate _template;
		public GeneralReportSearchOperation(SKDDatabaseService dataService, Guid selectedTemplateId, IEnumerable<Employee> employees)
			: base(dataService)
		{
			if (dataService == null)
				throw new ArgumentNullException("dataService");

			_dataService = dataService;
			_selectedTemplateId = selectedTemplateId;
			_employees = employees;
		}

		public override void Execute()
		{
			_template = _dataService.PassCardTemplateTranslator.GetSingle(_selectedTemplateId).Result;
			Initialize();
		}

		private void Initialize()
		{
			Result = new List<ReportDTO>();
			foreach (var employee in _employees)
			{
				var ds = CreateDataSetItem(employee);

				Result.Add(new ReportDTO
				{
					Report = _template.Front.Report,
					Data = ds,
					BackgroundImage = _template.Front.WatermarkImage != null ? _template.Front.WatermarkImage.ImageContent : null
				});

				if (_template.Back != null && _template.Back.Report != null)
				{
					Result.Add(new ReportDTO
					{
						Report = _template.Back.Report,
						Data = ds,
						BackgroundImage = _template.Back.WatermarkImage != null ? _template.Back.WatermarkImage.ImageContent : null
					});
				}
			}
		}
	}
}
