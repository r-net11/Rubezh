using System;
using StrazhAPI.Automation;
using StrazhAPI.Models.Automation;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class AddGlobalVariableDialogViewModel : SaveCancelDialogViewModel
	{
		private const string DefaulVariableName = "глобальная переменная";
		#region Properties

		private bool _isSaveWhenRestart;

		public bool IsSaveWhenRestart
		{
			get { return _isSaveWhenRestart; }
			set
			{
				if (_isSaveWhenRestart == value) return;
				_isSaveWhenRestart = value;
				OnPropertyChanged(() => IsSaveWhenRestart);
			}
		}
		public ExplicitValuesViewModel ExplicitValuesViewModel { get; protected set; }

		public GlobalVariable Variable { get; set; }

		public ObservableCollection<ExplicitTypeViewModel> ExplicitTypes { get; set; }

		ExplicitTypeViewModel _selectedExplicitType;
		public ExplicitTypeViewModel SelectedExplicitType
		{
			get { return _selectedExplicitType; }
			set
			{
				_selectedExplicitType = value;
				ExplicitValuesViewModel.ExplicitType = _selectedExplicitType.ExplicitType;

				if (_selectedExplicitType.ExplicitType == ExplicitType.Enum)
					ExplicitValuesViewModel.EnumType = _selectedExplicitType.EnumType;
				if (_selectedExplicitType.ExplicitType == ExplicitType.Object)
					ExplicitValuesViewModel.ObjectType = _selectedExplicitType.ObjectType;

				OnPropertyChanged(() => SelectedExplicitType);
				OnPropertyChanged(() => IsRealType);
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

		private bool _isReference;
		public bool IsReference
		{
			get { return _isReference; }
			set
			{
				_isReference = value;
				OnPropertyChanged(() => IsReference);
			}
		}
		public bool IsRealType
		{
			get
			{
				if (SelectedExplicitType == null)
					return false;

				if (SelectedExplicitType.ExplicitType != ExplicitType.Enum && SelectedExplicitType.ExplicitType != ExplicitType.Object)
					return true;

				return SelectedExplicitType.Parent != null;
			}
		}
		#endregion

		public AddGlobalVariableDialogViewModel(GlobalVariable variable)
		{
			Title = variable != null ? "Редактировать глобальную переменную" : "Добавить глобальную переменную";
			Name = variable != null ? variable.Name : DefaulVariableName;
			IsSaveWhenRestart = variable != null ? variable.IsSaveWhenRestart : default(bool);

			ExplicitValuesViewModel = new ExplicitValuesViewModel();
			ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel>(ProcedureHelper.BuildExplicitTypes(ProcedureHelper.GetEnumList<ExplicitType>(),
				ProcedureHelper.GetEnumList<EnumType>(), ProcedureHelper.GetEnumList<ObjectType>()));
			SelectedExplicitType = ExplicitTypes.FirstOrDefault();

			if (variable != null)
				Copy(variable);
			else
			{
				Variable = new GlobalVariable
				{
					VariableValue = new VariableValue()
				};
			}
		}

		void Copy(GlobalVariable variable)
		{
			ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel>(ProcedureHelper.BuildExplicitTypes(new List<ExplicitType> { variable.VariableValue.ExplicitType },
				new List<EnumType> { variable.VariableValue.EnumType }, new List<ObjectType> { variable.VariableValue.ObjectType }));
			var explicitTypeViewModel = ExplicitTypes.FirstOrDefault();

			if (explicitTypeViewModel != null)
			{
				SelectedExplicitType = explicitTypeViewModel.GetAllChildren().LastOrDefault();
				if (SelectedExplicitType != null) SelectedExplicitType.ExpandToThis();
			}

			ExplicitValuesViewModel = new ExplicitValuesViewModel(variable.VariableValue.ExplicitValue, variable.VariableValue.ExplicitValues, variable.VariableValue.ExplicitType, variable.VariableValue.EnumType, variable.VariableValue.ObjectType);
			Variable = variable;
		}

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning("Название не может быть пустым");
				return false;
			}

			Variable.IsReference = IsReference;
			Variable.Name = Name;
			Variable.IsSaveWhenRestart = IsSaveWhenRestart;
			Variable.VariableValue.EnumType = SelectedExplicitType.EnumType;
			Variable.VariableValue.ObjectType = SelectedExplicitType.ObjectType;
			Variable.VariableValue.ExplicitType = SelectedExplicitType.ExplicitType;
			Variable.VariableValue.ExplicitValue = ExplicitValuesViewModel.ExplicitValue.ExplicitValue;

			foreach(var explicitValue in ExplicitValuesViewModel.ExplicitValues)
				Variable.VariableValue.ExplicitValues.Add(explicitValue.ExplicitValue);

			return base.Save();
		}

		protected override bool CanSave()
		{
			return IsRealType;
		}
	}
}
