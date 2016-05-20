using Infrastructure.Automation;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class JournalStepViewModel : BaseStepViewModel
	{
		JournalStep JournalStep { get; set; }
		public ArgumentViewModel MessageArgument { get; set; }

		public JournalStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			JournalStep = (JournalStep)stepViewModel.Step;
			MessageArgument = new ArgumentViewModel(JournalStep.MessageArgument, stepViewModel.Update, null);
			ExplicitTypes = new ObservableCollection<ExplicitType>(AutomationHelper.GetEnumList<ExplicitType>());
			EnumTypes = AutomationHelper.GetEnumObs<EnumType>();
			ObjectTypes = AutomationHelper.GetEnumObs<ObjectType>();
		}

		public ObservableCollection<ExplicitType> ExplicitTypes { get; private set; }
		public ExplicitType ExplicitType
		{
			get { return JournalStep.ExplicitType; }
			set
			{
				JournalStep.ExplicitType = value;
				UpdateContent();
				OnPropertyChanged(() => ExplicitType);
			}
		}

		public ObservableCollection<EnumType> EnumTypes { get; private set; }
		public EnumType EnumType
		{
			get
			{
				return JournalStep.EnumType;
			}
			set
			{
				JournalStep.EnumType = value;
				UpdateContent();
				OnPropertyChanged(() => EnumType);
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }
		public ObjectType ObjectType
		{
			get
			{
				return JournalStep.ObjectType;
			}
			set
			{
				JournalStep.ObjectType = value;
				UpdateContent();
				OnPropertyChanged(() => ObjectType);
			}
		}

		public override void UpdateContent()
		{
			MessageArgument.Update(Procedure, ExplicitType, EnumType, ObjectType, false);
		}

		public override string Description
		{
			get
			{
				return "Сообщение: " + MessageArgument.Description;
			}
		}
	}
}