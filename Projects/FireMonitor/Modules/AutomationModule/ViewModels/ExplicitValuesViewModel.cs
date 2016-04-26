using FiresecAPI.Automation;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ExplicitValuesViewModel : BaseViewModel
	{
		public ExplicitValueViewModel ExplicitValue { get; private set; }
		public ObservableCollection<ExplicitValueViewModel> ExplicitValues { get; private set; }

		public ExplicitValuesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<ExplicitValueViewModel>(OnRemove);
			ChangeCommand = new RelayCommand<ExplicitValueViewModel>(OnChange);
			ExplicitValue = new ExplicitValueViewModel();
			ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
		}

		public ExplicitValuesViewModel(ExplicitValue explicitValue, List<ExplicitValue> explicitValues, ExplicitType explicitType, EnumType enumType, ObjectType objectType)
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<ExplicitValueViewModel>(OnRemove);
			ChangeCommand = new RelayCommand<ExplicitValueViewModel>(OnChange);
			ExplicitType = explicitType;
			EnumType = enumType;
			ObjectType = objectType;
			var newExplicitValue = new ExplicitValue();
			PropertyCopy.Copy(explicitValue, newExplicitValue);
			ExplicitValue = new ExplicitValueViewModel(newExplicitValue);
			ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
			foreach (var explicitVal in explicitValues)
			{
				var newExplicitVal = new ExplicitValue();
				PropertyCopy.Copy(explicitVal, newExplicitVal);
				ExplicitValues.Add(new ExplicitValueViewModel(newExplicitVal));
			}
		}

		ExplicitType _explicitType;
		public ExplicitType ExplicitType
		{
			get { return _explicitType; }
			set
			{
				_explicitType = value;
				ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
				OnPropertyChanged(() => ExplicitValues);
				OnPropertyChanged(() => ExplicitType);
			}
		}

		EnumType _enumType;
		public EnumType EnumType
		{
			get { return _enumType; }
			set
			{
				_enumType = value;
				ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
				OnPropertyChanged(() => ExplicitValues);
				OnPropertyChanged(() => EnumType);
			}
		}

		ObjectType _objectType;
		public ObjectType ObjectType
		{
			get { return _objectType; }
			set
			{
				_objectType = value;
				ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
				OnPropertyChanged(() => ExplicitValues);
				OnPropertyChanged(() => ObjectType);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var explicitValueViewModel = new ExplicitValueViewModel();
			if (ExplicitType == ExplicitType.Object)
				if (!ProcedureHelper.SelectObject(ObjectType, explicitValueViewModel))
					return;
			ExplicitValues.Add(explicitValueViewModel);
			OnPropertyChanged(() => ExplicitValues);
		}

		public RelayCommand<ExplicitValueViewModel> RemoveCommand { get; private set; }
		void OnRemove(ExplicitValueViewModel explicitValueViewModel)
		{
			ExplicitValues.Remove(explicitValueViewModel);
			OnPropertyChanged(() => ExplicitValues);
		}

		public RelayCommand<ExplicitValueViewModel> ChangeCommand { get; private set; }
		void OnChange(ExplicitValueViewModel explicitValueViewModel)
		{
			ProcedureHelper.SelectObject(ObjectType, ExplicitValue);
			OnPropertyChanged(() => ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
		}
	}
}
