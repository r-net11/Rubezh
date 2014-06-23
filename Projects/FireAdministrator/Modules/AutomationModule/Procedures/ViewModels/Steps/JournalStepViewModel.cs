using System;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class JournalStepViewModel : BaseViewModel, IStepViewModel
	{
		public JournalStepViewModel(ProcedureStep procedureStep)
		{
		}

		public void UpdateContent()
		{
			
		}

		public string Description
		{
			get { return ""; }
		}
	}
}