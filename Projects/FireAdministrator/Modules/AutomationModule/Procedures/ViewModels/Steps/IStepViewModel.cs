using System;

namespace AutomationModule.ViewModels
{
	public interface IStepViewModel
	{
		void UpdateContent();
		string Description { get; }
	}
}
