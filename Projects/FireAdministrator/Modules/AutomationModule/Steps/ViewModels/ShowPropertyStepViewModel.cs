﻿using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using Infrastructure.Automation;

namespace AutomationModule.ViewModels
{
	public class ShowPropertyStepViewModel : BaseStepViewModel
	{
		ShowPropertyArguments ShowPropertyArguments { get; set; }
		public ArgumentViewModel ObjectArgument { get; private set; }
		public ProcedureLayoutCollectionViewModel ProcedureLayoutCollectionViewModel { get; private set; }

		public ShowPropertyStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ShowPropertyArguments = stepViewModel.Step.ShowPropertyArguments;
			ObjectArgument = new ArgumentViewModel(ShowPropertyArguments.ObjectArgument, stepViewModel.Update, null);
			ObjectTypes = AutomationHelper.GetEnumObs<ObjectType>();
		}

		public override void UpdateContent()
		{
			ObjectArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType, isList: false);
			ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(ShowPropertyArguments.LayoutFilter);
			OnPropertyChanged(() => ProcedureLayoutCollectionViewModel);
		}

		public override string Description
		{
			get
			{
				return "Показать свойство объекта: " + ObjectArgument.Description;
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }
		public ObjectType ObjectType
		{
			get
			{
				return ShowPropertyArguments.ObjectType;
			}
			set
			{
				ShowPropertyArguments.ObjectType = value;
				UpdateContent();
				OnPropertyChanged(() => ObjectType);
			}
		}

		public bool ForAllClients
		{
			get { return ShowPropertyArguments.ForAllClients; }
			set
			{
				ShowPropertyArguments.ForAllClients = value;
				OnPropertyChanged(() => ForAllClients);
			}
		}
	}
}