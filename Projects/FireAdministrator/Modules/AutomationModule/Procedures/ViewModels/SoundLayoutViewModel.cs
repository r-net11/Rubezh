using FiresecAPI.Automation;
using FiresecAPI.Models.Layouts;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using LayoutModel = FiresecAPI.Models.Layouts.Layout;
using System;

namespace AutomationModule.ViewModels
{
	public class SoundLayoutViewModel : BaseStepViewModel
	{
		public LayoutModel Layout { get; private set; }
		public SoundArguments SoundArguments { get; private set; }

		public SoundLayoutViewModel(StepViewModel stepViewModel, LayoutModel layout) : base(stepViewModel)
		{
			Layout = layout;
			SoundArguments = stepViewModel.Step.SoundArguments;
		}

		public string Name
		{
			get { return Layout.Caption; }
		}

		public override string Description
		{
			get { return ""; }
		}

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				if (value && !SoundArguments.ProcedureLayoutCollection.LayoutsUIDs.Contains(Layout.UID))
					SoundArguments.ProcedureLayoutCollection.LayoutsUIDs.Add(Layout.UID);
				else if (!value)
					SoundArguments.ProcedureLayoutCollection.LayoutsUIDs.Remove(Layout.UID);
				OnPropertyChanged(() => IsChecked);
			}
		}
	}
}