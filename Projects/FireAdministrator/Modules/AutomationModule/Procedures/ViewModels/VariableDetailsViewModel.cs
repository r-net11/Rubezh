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
		public bool IsEditMode { get; set; }

		public VariableDetailsViewModel(Variable variable, string defaultName = "", string title = "")
		{
			Title = title;
			automationChanged = ServiceFactory.SaveService.AutomationChanged;
			VariableViewModel = new VariableViewModel(new Variable());
			VariableViewModel.Variable.Name = defaultName;			
			ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel>();
			foreach (var explicitType in ProcedureHelper.GetEnumObs<ExplicitType>())
				ExplicitTypes.Add(new ExplicitTypeViewModel(explicitType));
			SelectedExplicitType = ExplicitTypes.FirstOrDefault();
			EnumTypes = ProcedureHelper.GetEnumObs<EnumType>();
			ObjectTypes = ProcedureHelper.GetEnumObs<ObjectType>();
			if (variable != null)
				Copy(variable);
		}

		void Copy(Variable variable)
		{
			ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel> { new ExplicitTypeViewModel(variable.ExplicitType) };
			SelectedExplicitType = ExplicitTypes.FirstOrDefault();
			var newVariable = new Variable();
			newVariable.Name = variable.Name;
			newVariable.IsList = variable.IsList;
			newVariable.ExplicitType = variable.ExplicitType;
			newVariable.EnumType = variable.EnumType;
			newVariable.ObjectType = variable.ObjectType;
			PropertyCopy.Copy<ExplicitValue, ExplicitValue>(variable.DefaultExplicitValue, newVariable.DefaultExplicitValue);
			foreach (var defaultExplicitValue in variable.DefaultExplicitValues)
			{
				var newExplicitValue = new ExplicitValue();
				PropertyCopy.Copy<ExplicitValue, ExplicitValue>(defaultExplicitValue, newExplicitValue);
				newVariable.DefaultExplicitValues.Add(newExplicitValue);
			}
			VariableViewModel = new VariableViewModel(newVariable);
			IsEditMode = true;
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
				VariableViewModel.Variable.DefaultExplicitValues = new List<ExplicitValue>();
				VariableViewModel.ExplicitType = _selectedExplicitType.ExplicitType;
				VariableViewModel.Update();
				OnPropertyChanged(() => SelectedExplicitType);
			}
		}

		public ObservableCollection<EnumType> EnumTypes { get; private set; }
		public EnumType SelectedEnumType
		{
			get { return VariableViewModel.Variable.EnumType; }
			set
			{
				VariableViewModel.Variable.EnumType = value;
				VariableViewModel.ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
				VariableViewModel.Variable.DefaultExplicitValues = new List<ExplicitValue>();
				VariableViewModel.Update();
				OnPropertyChanged(() => SelectedEnumType);
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }
		public ObjectType SelectedObjectType
		{
			get { return VariableViewModel.Variable.ObjectType; }
			set
			{
				VariableViewModel.Variable.ObjectType = value;
				VariableViewModel.ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
				VariableViewModel.Variable.DefaultExplicitValues = new List<ExplicitValue>();
				VariableViewModel.Update();
				OnPropertyChanged(() => SelectedObjectType);
			}
		}

		public string Name
		{
			get { return VariableViewModel.Variable.Name; }
			set
			{
				VariableViewModel.Variable.Name = value;
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