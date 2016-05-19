using Infrastructure.Automation;
using RubezhAPI.Automation;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class SetValueStepViewModel : BaseStepViewModel
	{
		SetValueStep SetValueStep { get; set; }
		public ArgumentViewModel SourceArgument { get; private set; }
		public ArgumentViewModel TargetArgument { get; private set; }

		public SetValueStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			SetValueStep = (SetValueStep)stepViewModel.Step;
			SourceArgument = new ArgumentViewModel(SetValueStep.SourceArgument, stepViewModel.Update, UpdateContent);
			TargetArgument = new ArgumentViewModel(SetValueStep.TargetArgument, stepViewModel.Update, UpdateContent, false);
			ExplicitTypes = AutomationHelper.GetEnumObs<ExplicitType>();
			EnumTypes = AutomationHelper.GetEnumObs<EnumType>();
			ObjectTypes = AutomationHelper.GetEnumObs<ObjectType>();
			SelectedExplicitType = SetValueStep.ExplicitType;
		}

		public ObservableCollection<ExplicitType> ExplicitTypes { get; private set; }

		public ExplicitType SelectedExplicitType
		{
			get { return SetValueStep.ExplicitType; }
			set
			{
				SetValueStep.ExplicitType = value;
				SourceArgument.ExplicitType = value;
				OnPropertyChanged(() => SelectedExplicitType);
				UpdateContent();
			}
		}

		public ObservableCollection<EnumType> EnumTypes { get; private set; }

		public EnumType SelectedEnumType
		{
			get
			{
				return SetValueStep.EnumType;
			}
			set
			{
				SetValueStep.EnumType = value;
				SourceArgument.EnumType = value;
				UpdateContent();
				OnPropertyChanged(() => SelectedEnumType);
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }

		public ObjectType SelectedObjectType
		{
			get
			{
				return SetValueStep.ObjectType;
			}
			set
			{
				SetValueStep.ObjectType = value;
				SourceArgument.ObjectType = value;
				SourceArgument.ExplicitValue.IsEmpty = true;
				UpdateContent();
				OnPropertyChanged(() => SelectedObjectType);
			}
		}

		public override void UpdateContent()
		{
			TargetArgument.Update(Procedure, SelectedExplicitType, SelectedEnumType, SelectedObjectType, false);
			if (SelectedExplicitType == ExplicitType.String)
			{
				SourceArgument.Update(Procedure, AutomationHelper.GetEnumList<ExplicitType>(), isList: false);
				SourceArgument.ExplicitType = ExplicitType.String;
			}
			else if (SelectedExplicitType == ExplicitType.Integer || SelectedExplicitType == ExplicitType.Float)
			{
				SourceArgument.Update(Procedure, new List<ExplicitType> { ExplicitType.Integer, ExplicitType.Float }, isList: false);
				SourceArgument.ExplicitType = SelectedExplicitType;
			}
			else
				SourceArgument.Update(Procedure, SelectedExplicitType, SelectedEnumType, SelectedObjectType, false);
		}

		public override string Description
		{
			get { return TargetArgument.Description + " = " + SourceArgument.Description; }
		}
	}
}