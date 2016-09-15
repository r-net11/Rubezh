using ReportSystem.Api.Interfaces;
using System;

namespace StrazhAPI.Printing
{
	public class PrintReportSettings
	{
		public IPaperKindSetting PaperKindSetting { get; set; }
		public Guid? TemplateGuid { get; set; }
	}
}
