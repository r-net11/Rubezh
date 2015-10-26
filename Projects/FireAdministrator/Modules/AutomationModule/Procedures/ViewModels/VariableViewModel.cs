using RubezhAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using Infrastructure.Automation;

namespace AutomationModule.ViewModels
{
	public class VariableViewModel : BaseViewModel
	{
		public Variable Variable { get; set; }

		public VariableViewModel(Variable variable)
		{
			Variable = variable;
		}

		public string ValueDescription
		{
			get
			{
				var description = "";
				if (!Variable.IsList)
					description = AutomationHelper.GetStringValue(Variable.ExplicitValue, Variable.ExplicitType, Variable.EnumType);
				else
				{
					foreach (var explicitValue in Variable.ExplicitValues)
					{
						description += AutomationHelper.GetStringValue(explicitValue, Variable.ExplicitType, Variable.EnumType) + ", ";
					}
				}
				description = description.TrimEnd(',', ' ');
				return description;
			}
		}

		public string TypeDescription
		{
			get
			{
				if (Variable.ExplicitType == ExplicitType.Object)
					return Variable.ExplicitType.ToDescription() + " \\ " + Variable.ObjectType.ToDescription();
				if (Variable.ExplicitType == ExplicitType.Enum)
					return Variable.ExplicitType.ToDescription() + " \\ " + Variable.EnumType.ToDescription();
				return Variable.ExplicitType.ToDescription();
			}
		}

		public override string ToString()
		{
			if (Variable != null)
				return Variable.Name;
			return "";
		}

		public void Update()
		{
			OnPropertyChanged(() => Variable);
			OnPropertyChanged(() => ValueDescription);
			OnPropertyChanged(() => TypeDescription);
		}
	}
}