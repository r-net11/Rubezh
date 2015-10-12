using RubezhAPI.Automation;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Infrastructure.Automation;

namespace AutomationModule.ViewModels
{
	public class JournalStepViewModel : BaseStepViewModel
	{
		JournalArguments JournalArguments { get; set; }
		public ArgumentViewModel MessageArgument { get; set; }

		public JournalStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			JournalArguments = stepViewModel.Step.JournalArguments;
			MessageArgument = new ArgumentViewModel(JournalArguments.MessageArgument, stepViewModel.Update, null);
			ExplicitTypes = new ObservableCollection<ExplicitType>(AutomationHelper.GetEnumList<ExplicitType>().FindAll(x => x != ExplicitType.Object));
			EnumTypes = AutomationHelper.GetEnumObs<EnumType>(); 
		}

		public ObservableCollection<ExplicitType> ExplicitTypes { get; private set; }
		public ExplicitType ExplicitType
		{
			get { return JournalArguments.ExplicitType; }
			set
			{
				JournalArguments.ExplicitType = value;				
				UpdateContent();
				OnPropertyChanged(() => ExplicitType);
			}
		}

		public ObservableCollection<EnumType> EnumTypes { get; private set; }
		public EnumType EnumType
		{
			get
			{
				return JournalArguments.EnumType;
			}
			set
			{
				JournalArguments.EnumType = value;
				UpdateContent();
				OnPropertyChanged(() => EnumType);
			}
		}

		public override void UpdateContent()
		{
			MessageArgument.Update(Procedure, ExplicitType, EnumType, isList:false);
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