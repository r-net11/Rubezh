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
		public Variable Variable { get; private set; }
		public bool IsEditMode { get; private set; }
		public bool ForbidList { get; private set; }
		public VariableItemViewModel CurrentVariableItem { get; private set; }

		public VariableDetailsViewModel(Variable variable, string defaultName, string title, bool forbidList = false)
		{
			automationChanged = ServiceFactory.SaveService.AutomationChanged;
			Name = defaultName;
			Title = title;
			ForbidList = forbidList;
			OnPropertyChanged(() => ForbidList);
			SelectCommand = new RelayCommand(OnSelect);
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<VariableItemViewModel>(OnRemove);
			ChangeItemCommand = new RelayCommand<VariableItemViewModel>(OnChangeItem);

			Variable = new Variable();
			ObjectTypes = ProcedureHelper.GetEnumObs<ObjectType>();
			EnumTypes = ProcedureHelper.GetEnumObs<EnumType>();
			ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel>();
			foreach (var explicitType in ProcedureHelper.GetEnumObs<ExplicitType>())
			{
				ExplicitTypes.Add(new ExplicitTypeViewModel(explicitType));
			}
			SelectedExplicitType = ExplicitTypes.FirstOrDefault();
			CurrentVariableItem = new VariableItemViewModel(new VariableItem());
			VariableItems = new ObservableCollection<VariableItemViewModel>();
			if (variable != null)
				Copy(variable);
		}

		void Copy(Variable variable)
		{
			Name = variable.Name;
			IsList = variable.IsList;
			SelectedExplicitType = new ExplicitTypeViewModel(variable.ExplicitType);
			SelectedObjectType = variable.ObjectType;
			SelectedEnumType = variable.EnumType;
			ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel> { SelectedExplicitType };
			Variable.IsGlobal = variable.IsGlobal;
			IsEditMode = true;
			CurrentVariableItem = new VariableItemViewModel(variable.DefaultVariableItem);
			foreach (var variableItem in variable.VariableItems)
				VariableItems.Add(new VariableItemViewModel(variableItem));
		}

		public ObservableCollection<ExplicitTypeViewModel> ExplicitTypes { get; private set; }

		ExplicitTypeViewModel _selectedExplicitType;
		public ExplicitTypeViewModel SelectedExplicitType
		{
			get { return _selectedExplicitType; }
			set
			{
				_selectedExplicitType = value;
				VariableItems = new ObservableCollection<VariableItemViewModel>();
				OnPropertyChanged(() => VariableItems);
				OnPropertyChanged(() => SelectedExplicitType);
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
				VariableItems = new ObservableCollection<VariableItemViewModel>();
				CurrentVariableItem = new VariableItemViewModel(new VariableItem());
				OnPropertyChanged(() => CurrentVariableItem);
				OnPropertyChanged(() => VariableItems);
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
				VariableItems = new ObservableCollection<VariableItemViewModel>();
				OnPropertyChanged(() => VariableItems);
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
			SelectItem(CurrentVariableItem);
			OnPropertyChanged(()=>CurrentVariableItem);
		}

		public RelayCommand<VariableItemViewModel> ChangeItemCommand { get; private set; }
		public void OnChangeItem(VariableItemViewModel variableItemViewModel)
		{
			SelectItem(variableItemViewModel);
			OnPropertyChanged(() => VariableItems);
		}

		public ObservableCollection<VariableItemViewModel> VariableItems { get; private set; }

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var variableItemViewModel = new VariableItemViewModel(new VariableItem());
			SelectItem(variableItemViewModel);
			if (SelectedExplicitType.ExplicitType != ExplicitType.Object || !variableItemViewModel.IsEmpty)
				VariableItems.Add(variableItemViewModel);
			OnPropertyChanged(() => VariableItems);
		}

		public RelayCommand<VariableItemViewModel> RemoveCommand { get; private set; }
		void OnRemove(VariableItemViewModel variableItemViewModel)
		{
			if (variableItemViewModel == null)
				return;
			VariableItems.Remove(variableItemViewModel);
			OnPropertyChanged(() => VariableItems);
		}

		void SelectItem(VariableItemViewModel currentVariableItem = null)
		{
			if (currentVariableItem == null)
				currentVariableItem = new VariableItemViewModel(new VariableItem());
			if (SelectedExplicitType.ExplicitType != ExplicitType.Object)
				return;
			ProcedureHelper.SelectObject(SelectedObjectType, currentVariableItem);
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
			Variable.Name = Name;
			Variable.DefaultVariableItem = CurrentVariableItem.VariableItem;
			Variable.ExplicitType = SelectedExplicitType.ExplicitType;
			Variable.ObjectType = SelectedObjectType;
			Variable.EnumType = SelectedEnumType;
			Variable.IsList = IsList;
			Variable.VariableItems = new List<VariableItem>();
			foreach (var variableItemViewModel in VariableItems)
				Variable.VariableItems.Add(variableItemViewModel.VariableItem);
			return base.Save();
		}

		public class ExplicitTypeViewModel : BaseViewModel
		{
			public ExplicitType ExplicitType{ get; private set; }

			public ExplicitTypeViewModel(ExplicitType explicitType)
			{
				ExplicitType = explicitType;
			}

			public string Name
			{
				get { return ExplicitType.ToDescription(); }
			}
		}
	}
}