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
using FiresecAPI.GK;

namespace AutomationModule.ViewModels
{
	public class VariableDetailsViewModel : SaveCancelDialogViewModel
	{
		public Variable Variable { get; private set; }
		public bool IsEditMode { get; private set; }
		public VariableItemViewModel CurrentVariableItem { get; private set; }

		public VariableDetailsViewModel(Variable variable, string defaultName, string title)
		{
			Variable = new Variable();
			ObjectTypes = ProcedureHelper.GetEnumObs<ObjectType>();
			EnumTypes = ProcedureHelper.GetEnumObs<EnumType>();
			ValueTypes = ProcedureHelper.GetEnumObs<ValueType>();
			CurrentVariableItem = new VariableItemViewModel(new VariableItem());
			VariableItems = new List<VariableItemViewModel>();
			Name = defaultName;
			Title = title;
			if (variable != null)
				Copy(variable);
			SelectCommand = new RelayCommand(OnSelect);
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<VariableItemViewModel>(OnRemove);
			ChangeItemCommand = new RelayCommand<VariableItemViewModel>(OnChangeItem);
		}

		void Copy(Variable variable)
		{
			Name = variable.Name;
			IsList = variable.IsList;
			SelectedValueType = variable.ValueType;
			SelectedObjectType = variable.ObjectType;
			SelectedEnumType = variable.EnumType;
			ValueTypes = new ObservableCollection<ValueType> { variable.ValueType };
			Variable.IsGlobal = variable.IsGlobal;
			IsEditMode = true;
			CurrentVariableItem = new VariableItemViewModel(variable.DefaultVariableItem);
			foreach (var variableItem in variable.VariableItems)
				VariableItems.Add(new VariableItemViewModel(variableItem));
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

		public ObservableCollection<EnumType> EnumTypes { get; private set; }
		EnumType _selectedEnumType;
		public EnumType SelectedEnumType
		{
			get { return _selectedEnumType; }
			set
			{
				_selectedEnumType = value;
				VariableItems = new List<VariableItemViewModel>();
				UpdateVariableItems();
				OnPropertyChanged(() => SelectedEnumType);
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
			CurrentVariableItem = SelectItem(CurrentVariableItem);
			OnPropertyChanged(()=>CurrentVariableItem);
		}

		public RelayCommand<VariableItemViewModel> ChangeItemCommand { get; private set; }
		public void OnChangeItem(VariableItemViewModel variableItemViewModel)
		{
			variableItemViewModel.Initialize(SelectItem(variableItemViewModel).VariableItem);
			UpdateVariableItems();
		}
		
		#region VariableItems
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
		public ObservableCollection<VariableItemViewModel> VariableEnums
		{
			get { return new ObservableCollection<VariableItemViewModel>(VariableItems.FindAll(x => x.VariableItem.ValueType == ValueType.Enum)); }
		}
		#endregion

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
				currentVariableItem = new VariableItemViewModel(new VariableItem() {ValueType = SelectedValueType});
			if (SelectedValueType != ValueType.Object)
			{
				return currentVariableItem;
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
			OnPropertyChanged(() => VariableEnums);
		}

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning("Название не может быть пустым");
				return false;
			}
			Variable.Name = Name;
			Variable.DefaultVariableItem = CurrentVariableItem.VariableItem;
			Variable.ValueType = SelectedValueType;
			Variable.ObjectType = SelectedObjectType;
			Variable.EnumType = SelectedEnumType;
			Variable.IsList = IsList;
			Variable.VariableItems = new List<VariableItem>();
			foreach (var variableItemViewModel in VariableItems)
				Variable.VariableItems.Add(variableItemViewModel.VariableItem);
			return base.Save();
		}
	}
}