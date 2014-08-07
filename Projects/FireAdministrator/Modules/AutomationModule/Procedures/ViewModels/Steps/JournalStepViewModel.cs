using System;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class JournalStepViewModel : BaseViewModel, IStepViewModel
	{
		JournalArguments JournalArguments { get; set; }
		public Action UpdateDescriptionHandler { get; set; }

		public JournalStepViewModel(JournalArguments journalArguments, Action updateDescriptionHandler)
		{
			JournalArguments = journalArguments;
			UpdateDescriptionHandler = updateDescriptionHandler;
		}

		public string Message
		{
			get { return JournalArguments.Message; }
			set
			{
				JournalArguments.Message = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => Message);
				if (UpdateDescriptionHandler != null)
					UpdateDescriptionHandler();
			}
		}

		public void UpdateContent()
		{
			
		}

		public string Description
		{
			get { return Message; }
		}
	}
}