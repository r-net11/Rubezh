using System.Collections.ObjectModel;
using StrazhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class SetValueStepViewModel : BaseStepViewModel
	{
		SetValueArguments SetValueArguments { get; set; }
		public ArgumentViewModel SourceArgument { get; private set; }
		public ArgumentViewModel TargetArgument { get; private set; }

		public SetValueStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			SetValueArguments = stepViewModel.Step.SetValueArguments;
			SourceArgument = new ArgumentViewModel(SetValueArguments.SourceArgument, stepViewModel.Update, UpdateContent);
			TargetArgument = new ArgumentViewModel(SetValueArguments.TargetArgument, stepViewModel.Update, UpdateContent, false);
			ExplicitTypes = ProcedureHelper.GetEnumObs<ExplicitType>();
			EnumTypes = ProcedureHelper.GetEnumObs<EnumType>();
			ObjectTypes = ProcedureHelper.GetEnumObs<ObjectType>();
			SelectedExplicitType = SetValueArguments.ExplicitType;
		}

		public ObservableCollection<ExplicitType> ExplicitTypes { get; private set; }

		public ExplicitType SelectedExplicitType
		{
			get { return SetValueArguments.ExplicitType; }
			set
			{
				SetValueArguments.ExplicitType = value;
				OnPropertyChanged(() => SelectedExplicitType);
				UpdateContent();
			}
		}

		public ObservableCollection<EnumType> EnumTypes { get; private set; }

		public EnumType SelectedEnumType
		{
			get
			{
				return SetValueArguments.EnumType;
			}
			set
			{
				SetValueArguments.EnumType = value;
				UpdateContent();
				OnPropertyChanged(() => SelectedEnumType);
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }

		public ObjectType SelectedObjectType
		{
			get
			{
				return SetValueArguments.ObjectType;
			}
			set
			{
				SetValueArguments.ObjectType = value;
				UpdateContent();
				OnPropertyChanged(() => SelectedObjectType);
			}
		}

		public override void UpdateContent()
		{
			TargetArgument.Update(Procedure, SelectedExplicitType, SelectedEnumType, SelectedObjectType);
			if (SelectedExplicitType == ExplicitType.String)
			{
				SourceArgument.Update(Procedure, ProcedureHelper.GetEnumList<ExplicitType>().FindAll(x => x != ExplicitType.Object));
				SourceArgument.ExplicitType = ExplicitType.String;
			}
			else
				SourceArgument.Update(Procedure, SelectedExplicitType, SelectedEnumType, SelectedObjectType);
		}

		public override string Description
		{
			get { return TargetArgument.Description + " = " + SourceArgument.Description; }
		}
	}
}