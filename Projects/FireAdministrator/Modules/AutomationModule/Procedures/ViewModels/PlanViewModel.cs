using System.Collections.ObjectModel;
using RubezhAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class PlanViewModel : BaseViewModel
	{
		public Plan Plan { get; private set; }
		public PlanViewModel(Plan plan)
		{
			Plan = plan;
		}

		public string Caption
		{
			get { return Plan.Caption; }
		}
	}
}
