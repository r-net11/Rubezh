using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using ReportSystem.Api.Interfaces;
using StrazhAPI.SKD;

namespace StrazhAPI.Printing
{
	public class PrintReportSettings
	{
		public IPaperKindSetting PaperKindSetting { get; set; }
		public Guid? TemplateGuid { get; set; }
	}
}
