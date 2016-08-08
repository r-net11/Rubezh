using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using StrazhAPI.SKD;

namespace StrazhAPI.Printing
{
	public class PrintReportSettings
	{
	//	public bool IsUseAttachedTemplates { get; set; }
		public bool IsUseCutMarks { get; set; }

		public IPaperKindSetting PaperKindSetting { get; set; }
		public Guid? TemplateGuid { get; set; }
	}
}
