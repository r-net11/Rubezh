using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using PlansModule.Kursk.ViewModels;

namespace PlansModule.Kursk
{
	public class Module : ModuleBase
	{
		private PlanExtension _planExtension;
		private TanksViewModel _tanksViewModel;

		public override void CreateViewModels()
		{
			_tanksViewModel = new TanksViewModel();
			_planExtension = new PlanExtension(_tanksViewModel);
		}

		public override void Initialize()
		{
			_planExtension.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
		}
		public override string Name
		{
			get { return "Графические планы - Курск"; }
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new NavigationItem[0];
		}
	}
}