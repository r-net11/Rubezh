using System.Collections.ObjectModel;
using RubezhAPI.Automation;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Automation;
using System.Linq;
using RubezhClient;

namespace AutomationModule.ViewModels
{
	public class ProcedureDetailsViewModel : SaveCancelDialogViewModel
	{
		public Procedure Procedure { get; private set; }

		public ProcedureDetailsViewModel(Procedure procedure = null)
		{
			if (procedure == null)
			{
				Procedure = new Procedure { ContextType = ContextType.Server };
				var i = 0;
				do { i++; } while (ClientManager.SystemConfiguration.AutomationConfiguration.Procedures.Any(x => x.Name == Procedure.Name + i));
				Procedure.Name += i;
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
			StartWithServer = Procedure.StartWithServer;
			IsSync = Procedure.IsSync;
			TimeOut = Procedure.TimeOut;
			SelectedTimeType = Procedure.TimeType;
			SelectedContextType = Procedure.ContextType;
			TimeTypes = AutomationHelper.GetEnumObs<TimeType>();
			ContextTypes = AutomationHelper.GetEnumObs<ContextType>();
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

		bool _startWithServer;
		public bool StartWithServer
		{
			get { return _startWithServer; }
			set
			{
				_startWithServer = value;
				OnPropertyChanged(() => StartWithServer);
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
			if (ClientManager.SystemConfiguration.AutomationConfiguration.Procedures.Any(x => x.Name == Name && x.Uid != Procedure.Uid))
			{
				MessageBoxService.ShowWarning("Процедура с таким именем уже существует");
				return false;
			}
			Procedure.Name = Name;
			Procedure.Description = Description;
			Procedure.IsActive = IsActive;
			Procedure.StartWithServer = StartWithServer;
			Procedure.IsSync = IsSync;
			Procedure.TimeOut = TimeOut;
			Procedure.TimeType = SelectedTimeType;
			Procedure.ContextType = SelectedContextType;
			return true;
		}
	}
}