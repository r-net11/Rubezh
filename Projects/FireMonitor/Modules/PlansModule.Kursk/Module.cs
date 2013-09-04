using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using XFiresecAPI;

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
			ServiceFactory.Events.GetEvent<RegisterPlanPresenterEvent<Plan>>().Publish(_planPresenter);
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new NavigationItem[0];
		}

		public override string Name
		{
			get { return "Графические планы - Курск"; }
		}
	}
}