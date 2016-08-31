using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrazhAPI.Printing;
using StrazhAPI.SKD;
using StrazhDAL;

namespace FiresecService.Report.Operations
{
	public sealed class AppropriateReportSearchOperation : BaseReportSearchOperation
	{
		private readonly SKDDatabaseService _dataService;
		private readonly IEnumerable<Employee> _employees;

		public AppropriateReportSearchOperation(SKDDatabaseService dataService, IEnumerable<Employee> employees)
			: base(dataService)
		{
			if (dataService == null)
				throw new ArgumentNullException("dataService");
			if(employees == null)
				throw new ArgumentNullException("employees");

			_dataService = dataService;
			_employees = employees;
		}

		public override void Execute()
		{
			var allTemplates = _employees.SelectMany(x => x.Cards.Select(y => y.PassCardTemplateUID)).ToList();

			var templates = _dataService.PassCardTemplateTranslator.GetFullList(new PassCardTemplateFilter { UIDs = allTemplates.OfType<Guid>().ToList() })
				.Result
				.ToList();

			Initialize(templates);

		}

		private void Initialize(IEnumerable<PassCardTemplate> templates)
		{
			Result = new List<ReportDTO>();
			foreach (var template in templates)
			{
				foreach (var employee in _employees.Where(x => x.Cards.Any(card => card.PassCardTemplateUID == template.UID)))
				{
					var ds = CreateDataSetItem(employee);
					Result.Add(new ReportDTO
					{
						Report = template.Front.Report,
						Data = ds,
						BackgroundImage = template.Front.WatermarkImage.ImageContent
					});

					Result.Add(new ReportDTO
					{
						Report = template.Back.Report,
						Data = ds,
						BackgroundImage = template.Back.WatermarkImage.ImageContent
					});
				}
			}

		}
	}
}
