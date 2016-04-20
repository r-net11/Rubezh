using System.Collections.Generic;
using RubezhAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Navigation;
using Infrustructure.Plans.Events;

namespace PlansModule.Kursk
{
	public class Module : ModuleBase
	{
		private PlanExtension _planExtension;

		public override void CreateViewModels()
		{
			_planExtension = new PlanExtension();
		}

		public override void Initialize()
		{
			_planExtension.Initialize();
		}

		public override void RegisterPlanExtension()
		{
			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.PlansKursk; }
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new NavigationItem[0];
		}
	}
}