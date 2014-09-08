using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System;
using System.Collections.ObjectModel;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class RunProgrammStepViewModel : BaseStepViewModel
	{
		RunProgrammArguments RunProgrammArguments { get; set; }
		public Action UpdateDescriptionHandler { get; set; }
		Procedure Procedure { get; set; }

		public RunProgrammStepViewModel(RunProgrammArguments runProgrammArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			RunProgrammArguments = runProgrammArguments;
			UpdateDescriptionHandler = updateDescriptionHandler;
			Procedure = procedure;
			UpdateContent();
		}

		public string Path
		{
			get { return RunProgrammArguments.Path; }
			set
			{
				RunProgrammArguments.Path = value;
				OnPropertyChanged(() => Path);
			}
		}

		public string Parameters
		{
			get { return RunProgrammArguments.Parameters; }
			set
			{
				RunProgrammArguments.Parameters = value;
				OnPropertyChanged(() => Parameters);
			}
		}

		public void UpdateContent()
		{
			
		}

		public override string Description
		{
			get
			{
				return "";
			}
		}
	}
}
