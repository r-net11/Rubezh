using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using StrazhAPI.Automation;
using System.Collections.ObjectModel;
using Infrastructure;
using Localization.Automation.Common;

namespace AutomationModule.ViewModels
{
	public class VariableDetailsViewModel : SaveCancelDialogViewModel
	{
		private readonly bool _automationChanged;

		#region Properties
		public ExplicitValuesViewModel ExplicitValuesViewModel { get; protected set; }

		public Variable Variable { get; private set; }

		public bool IsEditMode { get; set; }

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

				if (SelectedExplicitType.ExplicitType != ExplicitType.Enum
					&& SelectedExplicitType.ExplicitType != ExplicitType.Object)
					return true;

				return SelectedExplicitType.Parent != null;
			}
		}
		#endregion

		public VariableDetailsViewModel(Variable variable, string defaultName = null, string title = null)
		{
			_automationChanged = ServiceFactory.SaveService.AutomationChanged;
			Title = title;
			Name = defaultName;
			ExplicitValuesViewModel = new ExplicitValuesViewModel();
			ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel>(ProcedureHelper.BuildExplicitTypes(ProcedureHelper.GetEnumList<ExplicitType>(),
				ProcedureHelper.GetEnumList<EnumType>(), ProcedureHelper.GetEnumList<ObjectType>()));
			SelectedExplicitType = ExplicitTypes.FirstOrDefault();
			if (variable != null)
				Copy(variable);
		}

		void Copy(Variable variable)
		{
			ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel>(ProcedureHelper.BuildExplicitTypes(new List<ExplicitType>{variable.ExplicitType},
				new List<EnumType> { variable.EnumType }, new List<ObjectType> { variable.ObjectType }));
			var explicitTypeViewModel = ExplicitTypes.FirstOrDefault();
			if (explicitTypeViewModel != null)
			{
				SelectedExplicitType = explicitTypeViewModel.GetAllChildren().LastOrDefault();
				if (SelectedExplicitType != null) SelectedExplicitType.ExpandToThis();
			}
			ExplicitValuesViewModel = new ExplicitValuesViewModel(variable.ExplicitValue, variable.ExplicitValues, variable.ExplicitType, variable.EnumType, variable.ObjectType);
			Name = variable.Name;
			IsEditMode = true;
			IsReference = variable.IsReference;
		}

		public override bool OnClosing(bool isCanceled)
		{
			ServiceFactory.SaveService.AutomationChanged = _automationChanged;
			return base.OnClosing(isCanceled);
		}

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning(CommonResources.SaveEmpty);
				return false;
			}
			Variable = new Variable
			{
				Name = Name,
				IsReference = IsReference,
				ExplicitType = SelectedExplicitType.ExplicitType,
				EnumType = SelectedExplicitType.EnumType,
				ObjectType = SelectedExplicitType.ObjectType,
				ExplicitValue = ExplicitValuesViewModel.ExplicitValue.ExplicitValue
			};
			foreach(var explicitValue in ExplicitValuesViewModel.ExplicitValues)
				Variable.ExplicitValues.Add(explicitValue.ExplicitValue);
			return base.Save();
		}

		protected override bool CanSave()
		{
			return IsRealType;
		}
	}
}