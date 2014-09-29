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
		public ExplicitValuesViewModel ExplicitValuesViewModel { get; protected set; }
		public Variable Variable { get; private set; }
		public bool IsEditMode { get; set; }

		public VariableDetailsViewModel(Variable variable, string defaultName = "", string title = "")
		{
			automationChanged = ServiceFactory.SaveService.AutomationChanged;
			Title = title;
			Name = defaultName;
			ExplicitValuesViewModel = new ExplicitValuesViewModel();
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
			ExplicitValuesViewModel = new ExplicitValuesViewModel(variable.DefaultExplicitValue, variable.DefaultExplicitValues, variable.IsList, variable.ExplicitType, variable.EnumType, variable.ObjectType);
			Name = variable.Name;
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
				ExplicitValuesViewModel.ExplicitType = _selectedExplicitType.ExplicitType;
				OnPropertyChanged(() => SelectedExplicitType);
			}
		}

		public ObservableCollection<EnumType> EnumTypes { get; private set; }
		public EnumType SelectedEnumType
		{
			get { return ExplicitValuesViewModel.EnumType; }
			set
			{
				ExplicitValuesViewModel.EnumType = value;
				OnPropertyChanged(() => SelectedEnumType);
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }
		public ObjectType SelectedObjectType
		{
			get { return ExplicitValuesViewModel.ObjectType; }
			set
			{
				ExplicitValuesViewModel.ObjectType = value;
				OnPropertyChanged(() => SelectedObjectType);
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		public bool IsList
		{
			get { return ExplicitValuesViewModel.IsList; }
			set
			{
				ExplicitValuesViewModel.IsList = value;
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
			Variable.Name = Name;
			Variable.IsList = IsList;
			Variable.ExplicitType = SelectedExplicitType.ExplicitType;
			Variable.EnumType = SelectedEnumType;
			Variable.ObjectType = SelectedObjectType;
			Variable.DefaultExplicitValue = ExplicitValuesViewModel.ExplicitValue.ExplicitValue;
			foreach(var explicitValue in ExplicitValuesViewModel.ExplicitValues)
				Variable.DefaultExplicitValues.Add(explicitValue.ExplicitValue);
			return base.Save();
		}
	}
}