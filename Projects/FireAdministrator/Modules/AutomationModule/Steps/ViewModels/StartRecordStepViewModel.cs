﻿using System.Collections.ObjectModel;
using RubezhAPI.Automation;
using RubezhAPI;

namespace AutomationModule.ViewModels
{
	public class StartRecordStepViewModel : BaseStepViewModel
	{
		StartRecordArguments StartRecordArguments { get; set; }
		public ArgumentViewModel CameraArgument { get; private set; }
		public ArgumentViewModel EventUIDArgument { get; set; }
		public ArgumentViewModel TimeoutArgument { get; set; }

		public StartRecordStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			StartRecordArguments = stepViewModel.Step.StartRecordArguments;
			EventUIDArgument = new ArgumentViewModel(StartRecordArguments.EventUIDArgument, stepViewModel.Update, UpdateContent);
			TimeoutArgument = new ArgumentViewModel(StartRecordArguments.TimeoutArgument, stepViewModel.Update, UpdateContent);
			CameraArgument = new ArgumentViewModel(StartRecordArguments.CameraArgument, stepViewModel.Update, null);
		}


		public override void UpdateContent()
		{
			CameraArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType.VideoDevice, isList: false);
			EventUIDArgument.Update(Procedure, ExplicitType.String);
			TimeoutArgument.Update(Procedure, ExplicitType.Integer);
		}

		public override string Description
		{
			get
			{
				return "Камера: " + CameraArgument.Description + " Идентификатор: " + EventUIDArgument.Description + " Таймаут: " + TimeoutArgument.Description;
			}
		}
	}
}