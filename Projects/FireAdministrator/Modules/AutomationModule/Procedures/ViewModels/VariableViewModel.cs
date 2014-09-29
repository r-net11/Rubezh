using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System.Linq;
using FiresecAPI.SKD;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure;
using System.Linq.Expressions;
using FiresecAPI;

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
					description = ProcedureHelper.GetStringValue(Variable.DefaultExplicitValue, Variable.ExplicitType, Variable.EnumType);
				else
				{
					foreach (var explicitValue in Variable.DefaultExplicitValues)
					{
						description += ProcedureHelper.GetStringValue(explicitValue, Variable.ExplicitType, Variable.EnumType) + ", ";
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

		public void Update()
		{
			OnPropertyChanged(() => Variable);
			OnPropertyChanged(() => ValueDescription);
			OnPropertyChanged(() => TypeDescription);
		}
	}
}