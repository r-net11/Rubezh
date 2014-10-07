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

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarningExtended("Название не может быть пустым");
				return false;
			}
			Procedure.Name = Name;
			Procedure.Description = Description;
			return true;
		}
	}
}