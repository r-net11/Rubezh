using System;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class SendMessageStepViewModel : BaseViewModel, IStepViewModel
	{
		SendMessageArguments SendMessageArguments { get; set; }
		public Action UpdateDescriptionHandler { get; set; }

		public SendMessageStepViewModel(SendMessageArguments sendMessageArguments, Action updateDescriptionHandler)
		{
			SendMessageArguments = sendMessageArguments;
			UpdateDescriptionHandler = updateDescriptionHandler;
		}

		public string Message
		{
			get { return SendMessageArguments.Message; }
			set
			{
				SendMessageArguments.Message = value;
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