using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Employees.Models;
using System;

namespace SKDModule.Employees.ViewModels.DialogWindows
{
	public class PrintingTemplatesDialogViewModel : SaveCancelDialogViewModel
	{

		public ReportSettings Settings { get; private set; }

		public PrintingTemplatesDialogViewModel(Guid organisationId)
		{
			Title = "Печать пропусков"; //TODO: Move to localization resources
			Settings = new ReportSettings(organisationId);
		}

		public Guid? GetSelectedTemplateId()
		{
			return Settings.SelectedTemplate == null ? (Guid?) null : Settings.SelectedTemplate.Item1;
		}
	}
}
