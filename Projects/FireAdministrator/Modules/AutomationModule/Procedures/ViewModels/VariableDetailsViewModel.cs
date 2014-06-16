using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class VariableDetailsViewModel : SaveCancelDialogViewModel
	{
		public VariableDetailsViewModel()
		{
			Title = "Добавить переменную";
			Initialize();
		}

		void Initialize()
		{
			Variables = new ObservableCollection<VariableViewModel>
			{
				new VariableViewModel { VariableType = VariableType.Boolean },
				new VariableViewModel { VariableType = VariableType.DateTime },
				new VariableViewModel { VariableType = VariableType.Integer },
				new VariableViewModel { VariableType = VariableType.Object },
				new VariableViewModel { VariableType = VariableType.String }
			};
		}
		
		VariableViewModel _selectedVariable;
		public VariableViewModel SelectedVariable
		{
			get { return _selectedVariable; }
			set
			{
				_selectedVariable = value;
				OnPropertyChanged(()=>SelectedVariable);
			}
		}

		ObservableCollection<VariableViewModel> _variables;
		public ObservableCollection<VariableViewModel> Variables
		{
			get { return _variables; }
			set
			{
				_variables = value;
				OnPropertyChanged(()=>Variables);
			}
		}

		protected override bool Save()
		{
			return base.Save();
		}
	}	
}