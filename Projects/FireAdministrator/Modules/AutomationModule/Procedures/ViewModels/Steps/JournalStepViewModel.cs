using System;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class JournalStepViewModel : BaseViewModel, IStepViewModel
	{
		JournalArguments JournalArguments { get; set; }

		public JournalStepViewModel(JournalArguments journalArguments)
		{
			JournalArguments = journalArguments;
		}

		public string Message
		{
			get { return JournalArguments.Message; }
			set
			{
				JournalArguments.Message = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => Message);
			}
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