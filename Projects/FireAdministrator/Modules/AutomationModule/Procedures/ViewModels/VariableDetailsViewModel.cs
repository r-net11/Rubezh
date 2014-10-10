using System.Linq;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System.Collections.ObjectModel;
using Infrastructure;

namespace AutomationModule.ViewModels
{
	public class VariableDetailsViewModel : SaveCancelDialogViewModel
	{
		readonly bool automationChanged;
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
			foreach (var enumType in ProcedureHelper.GetEnumObs<EnumType>())
			{
				var explicitTypeViewModel = new ExplicitTypeViewModel(enumType);
				var parent = ExplicitTypes.FirstOrDefault(x => x.ExplicitType == ExplicitType.Enum);
				if (parent != null)
				{
					parent.AddChild(explicitTypeViewModel);
				}
			}
			foreach (var objectType in ProcedureHelper.GetEnumObs<ObjectType>())
			{
				var explicitTypeViewModel = new ExplicitTypeViewModel(objectType);
				var parent = ExplicitTypes.FirstOrDefault(x => x.ExplicitType == ExplicitType.Object);
				if (parent != null)
				{
					parent.AddChild(explicitTypeViewModel);
				}
			}
			SelectedExplicitType = ExplicitTypes.FirstOrDefault();
			if (variable != null)
				Copy(variable);
		}

		void Copy(Variable variable)
		{
			ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel> { new ExplicitTypeViewModel(variable.ExplicitType) };
			var parent = ExplicitTypes.FirstOrDefault();
			SelectedExplicitType = parent;
			if (variable.ExplicitType == ExplicitType.Enum)
			{
				var explicitTypeViewModel = new ExplicitTypeViewModel(variable.EnumType);
				if (parent != null)
					parent.AddChild(explicitTypeViewModel);
				SelectedExplicitType = explicitTypeViewModel;
				SelectedExplicitType.ExpandToThis();
			}
			if (variable.ExplicitType == ExplicitType.Object)
			{
				var explicitTypeViewModel = new ExplicitTypeViewModel(variable.ObjectType);
				if (parent != null)
					parent.AddChild(explicitTypeViewModel);
				SelectedExplicitType = explicitTypeViewModel;
				SelectedExplicitType.ExpandToThis();
			}

			ExplicitValuesViewModel = new ExplicitValuesViewModel(variable.DefaultExplicitValue, variable.DefaultExplicitValues, variable.IsList, variable.ExplicitType, variable.EnumType, variable.ObjectType);
			Name = variable.Name;
			IsEditMode = true;
			IsReference = variable.IsReference;
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

		public bool IsList
		{
			get { return ExplicitValuesViewModel.IsList; }
			set
			{
				ExplicitValuesViewModel.IsList = value;
				OnPropertyChanged(() => IsList);
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
			Variable.IsReference = IsReference;
			Variable.ExplicitType = SelectedExplicitType.ExplicitType;
			Variable.EnumType = SelectedExplicitType.EnumType;
			Variable.ObjectType = SelectedExplicitType.ObjectType;
			Variable.DefaultExplicitValue = ExplicitValuesViewModel.ExplicitValue.ExplicitValue;
			foreach(var explicitValue in ExplicitValuesViewModel.ExplicitValues)
				Variable.DefaultExplicitValues.Add(explicitValue.ExplicitValue);
			return base.Save();
		}

		protected override bool CanSave()
		{
			return IsRealType;
		}

		public bool IsRealType
		{
			get
			{
				if (SelectedExplicitType == null)
					return false;
				if (SelectedExplicitType.ExplicitType == ExplicitType.Enum || SelectedExplicitType.ExplicitType == ExplicitType.Object)
					if (SelectedExplicitType.Parent == null)
						return false;
				return true;
			}
		}
	}
}