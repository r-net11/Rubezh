using System.Linq;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using FiresecClient;
using FiresecAPI.SKD;
using System;
using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecAPI;
using Infrastructure;

namespace AutomationModule.ViewModels
{
	public class VariableDetailsViewModel : SaveCancelDialogViewModel
	{
		bool automationChanged;
		public VariableViewModel VariableViewModel { get; protected set; }
		public Variable Variable { get; private set; }

		public VariableDetailsViewModel(Variable variable, string defaultName = "", string title = "")
		{
			Title = title;
			automationChanged = ServiceFactory.SaveService.AutomationChanged;
			if (this is ArgumentDetailsViewModel)
				VariableViewModel = new ArgumentViewModel(new Variable(), null);
			else
				VariableViewModel = new VariableViewModel(new Variable());
			VariableViewModel.Name = defaultName;			
			ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel>();
			foreach (var explicitType in ProcedureHelper.GetEnumObs<ExplicitType>())
				ExplicitTypes.Add(new ExplicitTypeViewModel(explicitType));
			SelectedExplicitType = ExplicitTypes.FirstOrDefault();
			if (variable != null)
				Copy(variable);
		}

		void Copy(Variable variable)
		{
			ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel> { new ExplicitTypeViewModel(variable.ExplicitType) };
			SelectedExplicitType = ExplicitTypes.FirstOrDefault();
			var newVariable = new Variable();
			PropertyCopy.Copy<Variable, Variable>(variable, newVariable);
			if (this is ArgumentDetailsViewModel)
				VariableViewModel = new ArgumentViewModel(newVariable, null);
			else
				VariableViewModel = new VariableViewModel(newVariable);
			VariableViewModel.IsEditMode = true;
		}

		public ObservableCollection<ExplicitTypeViewModel> ExplicitTypes { get; protected set; }
		ExplicitTypeViewModel _selectedExplicitType;
		public ExplicitTypeViewModel SelectedExplicitType
		{
			get { return _selectedExplicitType; }
			set
			{
				_selectedExplicitType = value;
				VariableViewModel.ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
				VariableViewModel.ExplicitType = _selectedExplicitType.ExplicitType;
				OnPropertyChanged(() => SelectedExplicitType);
			}
		}

		public string Name
		{
			get { return VariableViewModel.Name; }
			set
			{
				VariableViewModel.Name = value;
				OnPropertyChanged(() => Name);
			}
		}

		public bool IsList
		{
			get { return VariableViewModel.IsList; }
			set
			{
				VariableViewModel.IsList = value;
				OnPropertyChanged(() => IsList);
			}
		}

		public override bool OnClosing(bool isCanceled)
		{
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
			return base.OnClosing(isCanceled);
		}

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning("Название не может быть пустым");
				return false;
			}
			Variable = new Variable();
			PropertyCopy.Copy<Variable, Variable>(VariableViewModel.Variable, Variable);
			return base.Save();
		}
	}
}