using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ProcedureViewModel : BaseViewModel
	{
		public Procedure Procedure { get; private set; }

		public ProcedureViewModel(Procedure procedure)
		{
			Procedure = procedure;
			InputObjects = new ProcedureInputObjectsViewModel(procedure);
		}

		public string Name
		{
			get { return Procedure.Name; }
			set
			{
				Procedure.Name = value;
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public void Update(Procedure procedure)
		{
			Procedure = procedure;
			OnPropertyChanged("Name");
		}

		public ProcedureInputObjectsViewModel InputObjects { get; private set; }
		public ProcedureInputObjectsViewModel Steps { get; private set; }
	}
}