using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.Models.Layouts;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class LayoutProcedurePartViewModel : BaseViewModel
	{
		public LayoutProcedurePartViewModel(LayoutPartReferenceProperties properties)
		{
			Procedure = FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(item => item.Uid == properties.ReferenceUID);
			if (Procedure != null)
				Title = Procedure.Name;
			RunProcedureCommand = new RelayCommand(OnRunProcedure, CanRunProcedure);
		}

		public Procedure Procedure { get; private set; }
		public string Title { get; private set; }
		public RelayCommand RunProcedureCommand { get; private set; }
		private void OnRunProcedure()
		{
			ProcedureArgumentsViewModel.Run(Procedure);
		}
		private bool CanRunProcedure()
		{
			return Procedure != null;
		}
	}
}
