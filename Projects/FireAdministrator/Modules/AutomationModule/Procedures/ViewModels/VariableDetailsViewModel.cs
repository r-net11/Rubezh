using System.Linq;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using FiresecClient;
using FiresecAPI.SKD;
using System;
using ValueType = FiresecAPI.Automation.ValueType;
using System.Collections.Generic;

namespace AutomationModule.ViewModels
{
	public class VariableDetailsViewModel : SaveCancelDialogViewModel
	{
		public Variable Variable { get; private set; }
		public bool IsEditMode { get; private set; }

		public VariableDetailsViewModel(Variable variable, string defaultName, string title)
		{
			ValueTypes = ProcedureHelper.GetEnumObs<ValueType>();
			ObjectTypes = ProcedureHelper.GetEnumObs<ObjectType>();
			VariableItems = new List<VariableItemViewModel>();
			Variable = new Variable();
			SelectedVariableItem = new VariableItemViewModel(new VariableItem());
			Name = defaultName;
			if (variable != null)
				Copy(variable);
			Title = title;
			Initialize(variable);
		}

		void Copy(Variable variable)
		{
			Name = variable.Name;
			IsList = variable.IsList;
			Variable.IsGlobal = variable.IsGlobal;
			ValueTypes = new ObservableCollection<ValueType> { variable.ValueType };
			SelectedValueType = variable.ValueType;
			SelectedObjectType = variable.ObjectType;
			IsEditMode = true;
			DefaultIntValue = variable.DefaultIntValue;
			DefaultBoolValue = variable.DefaultBoolValue;
			DefaultDateTimeValue = variable.DefaultDateTimeValue;
			DefaultStringValue = variable.DefaultStringValue;
			SelectedVariableItem = new VariableItemViewModel(new VariableItem { ObjectUid = variable.ObjectUid });
			foreach (var variableItem in variable.VariableItems)
				VariableItems.Add(new VariableItemViewModel(variableItem));
		}

		void Initialize(Variable variable)
		{
			SelectCommand = new RelayCommand(OnSelect);
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<VariableItemViewModel>(OnRemove);
			ChangeItemCommand = new RelayCommand<VariableItemViewModel>(OnChangeItem);
		}

		public ObservableCollection<ValueType> ValueTypes { get; private set; }
		ValueType _selectedValueType;
		public ValueType SelectedValueType
		{
			get { return _selectedValueType; }
			set
			{
				_selectedValueType = value;
				VariableItems = new List<VariableItemViewModel>();
				UpdateVariableItems();
				OnPropertyChanged(() => SelectedValueType);
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }
		ObjectType _selectedObjectType;
		public ObjectType SelectedObjectType
		{
			get { return _selectedObjectType; }
			set
			{
				_selectedObjectType = value;
				VariableItems = new List<VariableItemViewModel>();
				UpdateVariableItems();
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

		bool _isList;
		public bool IsList
		{
			get { return _isList; }
			set
			{
				_isList = value;
				OnPropertyChanged(() => IsList);
			}
		}

		public RelayCommand SelectCommand { get; private set; }
		void OnSelect()
		{
			SelectedVariableItem = SelectItem(SelectedVariableItem);
		}

		public RelayCommand<VariableItemViewModel> ChangeItemCommand { get; private set; }
		public void OnChangeItem(VariableItemViewModel variableItemViewModel)
		{
			variableItemViewModel.Initialize(SelectItem(variableItemViewModel).VariableItem);
			UpdateVariableItems();
		}

		public List<VariableItemViewModel> VariableItems { get; private set; }
		public ObservableCollection<VariableItemViewModel> VariableObjects
		{
			get { return new ObservableCollection<VariableItemViewModel>(VariableItems.FindAll(x => x.VariableItem.ValueType == ValueType.Object)); }
		}
		public ObservableCollection<VariableItemViewModel> VariableBools
		{
			get { return new ObservableCollection<VariableItemViewModel>(VariableItems.FindAll(x => x.VariableItem.ValueType == ValueType.Boolean)); }
		}
		public ObservableCollection<VariableItemViewModel> VariableDateTimes
		{
			get { return new ObservableCollection<VariableItemViewModel>(VariableItems.FindAll(x => x.VariableItem.ValueType == ValueType.DateTime)); }
		}
		public ObservableCollection<VariableItemViewModel> VariableIntegers
		{
			get { return new ObservableCollection<VariableItemViewModel>(VariableItems.FindAll(x => x.VariableItem.ValueType == ValueType.Integer)); }
		}
		public ObservableCollection<VariableItemViewModel> VariableStrings
		{
			get { return new ObservableCollection<VariableItemViewModel>(VariableItems.FindAll(x => x.VariableItem.ValueType == ValueType.String)); }
		}

		VariableItemViewModel _selectedVariableItem;
		public VariableItemViewModel SelectedVariableItem
		{
			get { return _selectedVariableItem; }
			set
			{
				_selectedVariableItem = value;
				if (value != null)
					Variable.ObjectUid = _selectedVariableItem.VariableItem.ObjectUid;
				OnPropertyChanged(() => SelectedVariableItem);
			}
		}

		bool _defaultBoolValue;
		public bool DefaultBoolValue
		{
			get { return _defaultBoolValue; }
			set
			{
				_defaultBoolValue = value;
				OnPropertyChanged(() => DefaultBoolValue);
			}
		}

		DateTime _defaultDateTimeValue;
		public DateTime DefaultDateTimeValue
		{
			get { return _defaultDateTimeValue; }
			set
			{
				_defaultDateTimeValue = value;
				OnPropertyChanged(() => DefaultDateTimeValue);
			}
		}

		int _defaultIntValue;
		public int DefaultIntValue
		{
			get { return _defaultIntValue; }
			set
			{
				_defaultIntValue = value;
				OnPropertyChanged(() => DefaultIntValue);
			}
		}

		string _defaultStringValue;
		public string DefaultStringValue
		{
			get { return _defaultStringValue; }
			set
			{
				_defaultStringValue = value;
				OnPropertyChanged(() => DefaultStringValue);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var variableItemViewModel = SelectItem();
			if (variableItemViewModel.VariableItem.ValueType != ValueType.Object || !variableItemViewModel.IsEmpty)
				VariableItems.Add(variableItemViewModel);
			UpdateVariableItems();
		}

		public RelayCommand<VariableItemViewModel> RemoveCommand { get; private set; }
		void OnRemove(VariableItemViewModel variableItemViewModel)
		{
			if (variableItemViewModel == null)
				return;
			VariableItems.Remove(variableItemViewModel);
			UpdateVariableItems();
		}

		VariableItemViewModel SelectItem(VariableItemViewModel currentVariableItem = null)
		{
			if (currentVariableItem == null)
				currentVariableItem = new VariableItemViewModel(new VariableItem());
			if (SelectedValueType != ValueType.Object)
			{
				var variableItemViewModel = new VariableItemViewModel(new VariableItem { ValueType = SelectedValueType });
				variableItemViewModel.SelectedBoolValue = currentVariableItem.SelectedBoolValue;
				return variableItemViewModel;
			}

			return ProcedureHelper.SelectObject(SelectedObjectType, currentVariableItem);
		}

		void UpdateVariableItems()
		{
			OnPropertyChanged(() => VariableObjects);
			OnPropertyChanged(() => VariableBools);
			OnPropertyChanged(() => VariableDateTimes);
			OnPropertyChanged(() => VariableIntegers);
			OnPropertyChanged(() => VariableStrings);
		}

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning("Название не может быть пустым");
				return false;
			}
			Variable.Name = Name;
			Variable.DefaultBoolValue = DefaultBoolValue;
			Variable.DefaultDateTimeValue = DefaultDateTimeValue;
			Variable.DefaultIntValue = DefaultIntValue;
			Variable.DefaultStringValue = DefaultStringValue;
			Variable.ValueType = SelectedValueType;
			Variable.ObjectType = SelectedObjectType;
			Variable.IsList = IsList;
			Variable.VariableItems = new List<VariableItem>();
			foreach (var variableItemViewModel in VariableItems)
				Variable.VariableItems.Add(variableItemViewModel.VariableItem);
			return base.Save();
		}
	}
}