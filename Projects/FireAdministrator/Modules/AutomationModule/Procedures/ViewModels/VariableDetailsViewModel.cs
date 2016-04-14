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
		public ExplicitValueViewModel ExplicitValueViewModel { get; protected set; }
		public Variable Variable { get; private set; }
		public bool IsEditMode { get; set; }

		public VariableDetailsViewModel(Variable variable, List<Variable> availableVariables, string title = "", bool isGlobal = false)
		{
			automationChanged = ServiceFactory.SaveService.AutomationChanged;
			_availableVariables = availableVariables;

			Variable = new Variable { IsGlobal = isGlobal };
			if (variable == null)
			{
				ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel>(ProcedureHelper.BuildExplicitTypes(AutomationHelper.GetEnumList<ExplicitType>(),
				AutomationHelper.GetEnumList<EnumType>(), AutomationHelper.GetEnumList<ObjectType>()));
				_selectedExplicitType = ExplicitTypes.FirstOrDefault();
			}
			else
			{
				Variable.Uid = variable.Uid;
				Variable.Value = variable.Value;
				Variable.IsReference = variable.IsReference;
				Variable.IsGlobal = variable.IsGlobal;
				Variable.IsList = variable.IsList;
				Variable.ObjectType = variable.ObjectType;
				Name = variable.Name;
				IsEditMode = true;
				_isNameEdited = true;

				ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel>(ProcedureHelper.BuildExplicitTypes(new List<ExplicitType> { variable.ExplicitType },
				new List<EnumType> { variable.EnumType }, new List<ObjectType> { variable.ObjectType }));
				var explicitTypeViewModel = ExplicitTypes.FirstOrDefault();
				if (explicitTypeViewModel != null)
				{
					_selectedExplicitType = explicitTypeViewModel.GetAllChildren().LastOrDefault();
					if (_selectedExplicitType != null) _selectedExplicitType.ExpandToThis();
				}
			}

			SelectedContextType = Variable.ContextType;

			Title = title;
			IsGlobal = isGlobal;
			ContextTypes = AutomationHelper.GetEnumObs<ContextType>();
			ExplicitValueViewModel = new ExplicitValueViewModel((ExplicitValue)Variable);

		}

		public bool IsGlobal
		{
			get { return Variable.IsGlobal; }
			set
			{
				Variable.IsGlobal = value;
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
				Variable.ExplicitType = SelectedExplicitType.ExplicitType;
				ExplicitValueViewModel.ExplicitType = SelectedExplicitType.ExplicitType;
				ExplicitValueViewModel.EnumType = SelectedExplicitType.EnumType;
				ExplicitValueViewModel.ObjectType = SelectedExplicitType.ObjectType;
				ExplicitValueViewModel.OnPropertyChanged("IsEmpty");
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
			get { return Variable.IsList; }
			set
			{
				Variable.IsList = value;
				ExplicitValueViewModel.IsList = value;
				GenerateName();
				OnPropertyChanged(() => IsList);
			}
		}

		public bool IsReference
		{
			get { return Variable.IsReference; }
			set
			{
				Variable.IsReference = value;
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

			Variable.Name = _name;
			if (IsList)
				Variable.Value = ExplicitValueViewModel.GetListValue();

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
					_name = "целое";
					break;
				case ExplicitType.Float:
					_name = "вещественное";
					break;
				case ExplicitType.Boolean:
					_name = "логическое";
					break;
				case ExplicitType.DateTime:
					_name = "датаВремя";
					break;
				case ExplicitType.String:
					_name = "строка";
					break;

				case ExplicitType.Enum:
					switch (SelectedExplicitType.EnumType)
					{
						case EnumType.StateType:
							_name = "состояние";
							break;
						case EnumType.DriverType:
							_name = "типУстройства";
							break;
						case EnumType.PermissionType:
							_name = "праваПользователя";
							break;
						case EnumType.JournalEventNameType:
							_name = "названиеСобытия";
							break;
						case EnumType.JournalEventDescriptionType:
							_name = "уточнениеСобытия";
							break;
						case EnumType.JournalObjectType:
							_name = "типОбъекта";
							break;
						case EnumType.ColorType:
							_name = "цвет";
							break;
					}
					break;

				case ExplicitType.Object:
					switch (SelectedExplicitType.ObjectType)
					{
						case ObjectType.Device:
							_name = "устройство";
							break;
						case ObjectType.Zone:
							_name = "пожарнаяЗона";
							break;
						case ObjectType.Direction:
							_name = "направление";
							break;
						case ObjectType.Delay:
							_name = "задержка";
							break;
						case ObjectType.GuardZone:
							_name = "охраннаяЗона";
							break;
						case ObjectType.VideoDevice:
							_name = "видеоустройство";
							break;
						case ObjectType.GKDoor:
							_name = "точкаДоступа";
							break;
						case ObjectType.PumpStation:
							_name = "насоснаяСтанция";
							break;
						case ObjectType.MPT:
							_name = "мпт";
							break;
						case ObjectType.Organisation:
							_name = "организация";
							break;
					}
					break;
			}

			if (IsGlobal)
				_name = _name[0].ToString().ToUpper() + _name.Substring(1);
			if (IsList)
				_name = _name += "Список";

			int i = 0;
			do { i++; } while (_availableVariables.Any(x => x.Name == _name + i));

			_name += i;

			OnPropertyChanged(() => Name);
		}
	}
}