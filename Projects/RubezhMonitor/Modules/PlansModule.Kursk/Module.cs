using System.Collections.Generic;
using RubezhAPI.GK;
using RubezhAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Plans.Events;

namespace PlansModule.Kursk
{
	public class Module : ModuleBase
	{
		private PlanPresenter _planPresenter;

		public Module()
		{
			_planPresenter = new PlanPresenter();
		}

		public override void RegisterResource()
		{
		}
		public override void CreateViewModels()
		{
		}

		public override void Initialize()
		{
			_planPresenter.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanPresenterEvent<Plan, XStateClass>>().Publish(_planPresenter);
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new NavigationItem[0];
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.PlansKursk; }
		}
	}
}