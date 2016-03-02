using Infrastructure;
using Infrastructure.Automation;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class VariableDetailsViewModel : SaveCancelDialogViewModel
	{
		bool _isNameEdited;
		List<Variable> _availableVariables;
		readonly bool automationChanged;
		public ExplicitValuesViewModel ExplicitValuesViewModel { get; protected set; }
		public Variable Variable { get; private set; }
		public bool IsEditMode { get; set; }

		public VariableDetailsViewModel(Variable variable, List<Variable> availableVariables, string title = "", bool isGlobal = false)
		{
			automationChanged = ServiceFactory.SaveService.AutomationChanged;
			_availableVariables = availableVariables;
			Variable = variable;
			Title = title;
			IsGlobal = isGlobal;
			ContextTypes = AutomationHelper.GetEnumObs<ContextType>();
			ExplicitValuesViewModel = new ExplicitValuesViewModel();
			ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel>(ProcedureHelper.BuildExplicitTypes(AutomationHelper.GetEnumList<ExplicitType>(),
				AutomationHelper.GetEnumList<EnumType>(), AutomationHelper.GetEnumList<ObjectType>()));
			SelectedExplicitType = ExplicitTypes.FirstOrDefault();
			if (variable != null)
				Copy(variable);
		}

		void Copy(Variable variable)
		{
			ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel>(ProcedureHelper.BuildExplicitTypes(new List<ExplicitType> { variable.ExplicitType },
				new List<EnumType> { variable.EnumType }, new List<ObjectType> { variable.ObjectType }));
			var explicitTypeViewModel = ExplicitTypes.FirstOrDefault();
			if (explicitTypeViewModel != null)
			{
				SelectedExplicitType = explicitTypeViewModel.GetAllChildren().LastOrDefault();
				if (SelectedExplicitType != null) SelectedExplicitType.ExpandToThis();
			}
			ExplicitValuesViewModel = new ExplicitValuesViewModel(variable.ExplicitValue, variable.ExplicitValues, variable.IsList, variable.ExplicitType, variable.EnumType, variable.ObjectType);
			Name = variable.Name;
			IsEditMode = true;
			IsReference = variable.IsReference;
			IsGlobal = variable.IsGlobal;
			SelectedContextType = variable.ContextType;
		}

		bool _isGlobal;
		public bool IsGlobal
		{
			get { return _isGlobal; }
			set
			{
				_isGlobal = value;
				OnPropertyChanged(() => IsGlobal);
			}
		}

		public ObservableCollection<ContextType> ContextTypes { get; private set; }
		ContextType _selectedContextType;
		public ContextType SelectedContextType
		{
			get { return _selectedContextType; }
			set
			{
				_selectedContextType = value;
				OnPropertyChanged(() => SelectedContextType);
			}
		}

		public ObservableCollection<ExplicitTypeViewModel> ExplicitTypes { get; set; }
		ExplicitTypeViewModel _selectedExplicitType;
		public ExplicitTypeViewModel SelectedExplicitType
		{
			get { return _selectedExplicitType; }
			set
			{
				_selectedExplicitType = value;
				GenerateName();
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
				_isNameEdited = true;
				OnPropertyChanged(() => Name);
			}
		}

		public bool IsList
		{
			get { return ExplicitValuesViewModel.IsList; }
			set
			{
				ExplicitValuesViewModel.IsList = value;
				GenerateName();
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
			if (_availableVariables.Any(x => x.Name == Name && (Variable == null || x.Uid != Variable.Uid)))
			{
				MessageBoxService.ShowWarning("Переменная с таким именем уже существует");
				return false;
			}
			Variable = new Variable();
			Variable.Name = Name;
			Variable.IsList = IsList;
			Variable.IsGlobal = IsGlobal;
			Variable.ContextType = SelectedContextType;
			Variable.IsReference = IsReference;
			Variable.ExplicitType = SelectedExplicitType.ExplicitType;
			Variable.EnumType = SelectedExplicitType.EnumType;
			Variable.ObjectType = SelectedExplicitType.ObjectType;
			Variable.ExplicitValue = ExplicitValuesViewModel.ExplicitValue.ExplicitValue;
			foreach (var explicitValue in ExplicitValuesViewModel.ExplicitValues)
				Variable.ExplicitValues.Add(explicitValue.ExplicitValue);
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

		void GenerateName()
		{
			if (_isNameEdited || SelectedExplicitType == null || !SelectedExplicitType.IsRealType)
				return;

			switch (SelectedExplicitType.ExplicitType)
			{
				case ExplicitType.Integer:
					_name = "Целое";
					break;
				case ExplicitType.Float:
					_name = "Вещественное";
					break;
				case ExplicitType.Boolean:
					_name = "Логическое";
					break;
				case ExplicitType.DateTime:
					_name = "ДатаВремя";
					break;
				case ExplicitType.String:
					_name = "Строка";
					break;

				case ExplicitType.Enum:
					switch (SelectedExplicitType.EnumType)
					{
						case EnumType.StateType:
							_name = "Состояние";
							break;
						case EnumType.DriverType:
							_name = "ТипУстройства";
							break;
						case EnumType.PermissionType:
							_name = "ПраваПользователя";
							break;
						case EnumType.JournalEventNameType:
							_name = "НазваниеСобытия";
							break;
						case EnumType.JournalEventDescriptionType:
							_name = "УточнениеСобытия";
							break;
						case EnumType.JournalObjectType:
							_name = "ТипОбъекта";
							break;
						case EnumType.ColorType:
							_name = "Цвет";
							break;
					}
					break;

				case ExplicitType.Object:
					switch (SelectedExplicitType.ObjectType)
					{
						case ObjectType.Device:
							_name = "Устройство";
							break;
						case ObjectType.Zone:
							_name = "ПожарнаяЗона";
							break;
						case ObjectType.Direction:
							_name = "Направление";
							break;
						case ObjectType.Delay:
							_name = "Задержка";
							break;
						case ObjectType.GuardZone:
							_name = "ОхраннаяЗона";
							break;
						case ObjectType.VideoDevice:
							_name = "Видеоустройство";
							break;
						case ObjectType.GKDoor:
							_name = "ТочкаДоступа";
							break;
						case ObjectType.PumpStation:
							_name = "НасоснаяСтанция";
							break;
						case ObjectType.MPT:
							_name = "МПТ";
							break;
						case ObjectType.Organisation:
							_name = "Организация";
							break;
					}
					break;
			}

			if (IsList)
				_name = "Список" + _name;

			if (!IsGlobal)
				_name = _name[0].ToString().ToLower() + _name.Substring(1);

			int i = 0;
			do { i++; } while (_availableVariables.Any(x => x.Name == _name + i));

			_name += i;

			OnPropertyChanged(() => Name);
		}
	}
}