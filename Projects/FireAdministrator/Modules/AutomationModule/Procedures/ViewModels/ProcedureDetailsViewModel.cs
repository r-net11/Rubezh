using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ProcedureDetailsViewModel : SaveCancelDialogViewModel
	{
		public Procedure Procedure { get; private set; }

		public ProcedureDetailsViewModel(Procedure procedure = null)
		{
			if (procedure == null)
			{
				Procedure = new Procedure();
				Title = "Создание процедуры";
			}
			else
			{
				Procedure = procedure;
				Title = "Редактирование процедуры";
			}
			Name = Procedure.Name;
			Description = Procedure.Description;
			IsActive = Procedure.IsActive;
			IsSync = Procedure.IsSync;
			TimeOut = Procedure.TimeOut;
			SelectedTimeType = Procedure.TimeType;
			TimeTypes = ProcedureHelper.GetEnumObs<TimeType>();
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

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				_isActive = value;
				OnPropertyChanged(() => IsActive);
			}
		}

		bool _isSync;
		public bool IsSync
		{
			get { return _isSync; }
			set
			{
				_isSync = value;
				OnPropertyChanged(() => IsSync);
			}
		}

		int _timeOut;
		public int TimeOut
		{
			get { return _timeOut; }
			set
			{
				_timeOut = value;
				OnPropertyChanged(() => TimeOut);
			}
		}

		public ObservableCollection<TimeType> TimeTypes { get; private set; }
		TimeType _selectedTimeType;
		public TimeType SelectedTimeType
		{
			get { return _selectedTimeType; }
			set
			{
				_selectedTimeType = value;
				OnPropertyChanged(() => SelectedTimeType);
			}
		}

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning("Название не может быть пустым");
				return false;
			}
			Procedure.Name = Name;
			Procedure.Description = Description;
			Procedure.IsActive = IsActive;
			Procedure.IsSync = IsSync;
			Procedure.TimeOut = TimeOut;
			Procedure.TimeType = SelectedTimeType;
			return true;
		}
	}
}