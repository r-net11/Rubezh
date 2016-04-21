using AutomationModule.Plans;
using AutomationModule.ViewModels;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Plans.Events;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using System.Collections.Generic;

namespace AutomationModule
{
	public class AutomationModuleLoader : ModuleBase, ILayoutProviderModule
	{
		PlanPresenter _planPresenter;
		NavigationItem _proceduresNavigationItem;

		public override void CreateViewModels()
		{
			_planPresenter = new PlanPresenter();
			ProcessShedule();
		}

		void ProcessShedule()
		{
		}

		public override void Initialize()
		{
			_planPresenter.Initialize();
			ServiceFactoryBase.Events.GetEvent<RegisterPlanPresenterEvent<Plan, XStateClass>>().Publish(_planPresenter);
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>();
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Automation; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			ServiceFactoryBase.ResourceService.AddResource(GetType().Assembly, "DataTemplates/Dictionary.xaml");
		}
		public override void Dispose()
		{
		}

		#region ILayoutProviderModule Members
		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.AutomationProcedure, "Процедура", "Procedure.png", p => new LayoutProcedurePartViewModel((LayoutPartProcedureProperties)p));
			yield return new LayoutPartPresenter(LayoutPartIdentities.TextBlock, "Метка", "Text.png", p => new LayoutTextBlockPartViewModel((LayoutPartTextProperties)p));
			yield return new LayoutPartPresenter(LayoutPartIdentities.TextBox, "Текстовое поле", "Text.png", p => new LayoutTextBoxPartViewModel((LayoutPartTextProperties)p));
		}
		#endregion
	}
}