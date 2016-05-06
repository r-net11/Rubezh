using StrazhAPI.Automation;
using StrazhAPI.Models.Automation;
using Infrastructure.Common.Windows.ViewModels;
using StrazhAPI;

namespace AutomationModule.ViewModels
{
	public class VariableViewModel : BaseViewModel
	{
		public IVariable Variable { get; set; }

		public VariableViewModel(IVariable variable)
		{
			Variable = variable;
		}

		public string ValueDescription
		{
			get
			{
				var description = ProcedureHelper.GetStringValue(Variable.VariableValue.ExplicitValue, Variable.VariableValue.ExplicitType, Variable.VariableValue.EnumType);
				description = description.TrimEnd(',', ' ');
				return description;
			}
		}

		public string TypeDescription
		{
			get
			{
				if (Variable.VariableValue.ExplicitType == ExplicitType.Object)
					return Variable.VariableValue.ExplicitType.ToDescription() + " \\ " + Variable.VariableValue.ObjectType.ToDescription();
				if (Variable.VariableValue.ExplicitType == ExplicitType.Enum)
					return Variable.VariableValue.ExplicitType.ToDescription() + " \\ " + Variable.VariableValue.EnumType.ToDescription();
				return Variable.VariableValue.ExplicitType.ToDescription();
			}
		}

		public override string ToString()
		{
			return Variable != null ? Variable.Name : string.Empty;
		}

		public void Update()
		{
			OnPropertyChanged(() => Variable);
			OnPropertyChanged(() => ValueDescription);
			OnPropertyChanged(() => TypeDescription);
		}
	}
}