using AutomationModule.Events;
using AutomationModule.Plans.InstrumentAdorners;
using AutomationModule.Plans.ViewModels;
using AutomationModule.ViewModels;
using Common;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.Events;
using Infrastructure.Plans.Services;
using Infrastructure.Plans.ViewModels;
using Infrastructuret.Plans;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomationModule.Plans
{
	class AutomationPlanExtension : BasePlanExtension
	{
		public static AutomationPlanExtension Instance { get; private set; }

		private ProceduresViewModel _proceduresViewModel;
		private IEnumerable<IInstrument> _instruments;

		public AutomationPlanExtension(ProceduresViewModel proceduresViewModel)
		{
			Instance = this;
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			_proceduresViewModel = proceduresViewModel;
			_instruments = null;
			Cache.Add<Procedure>(() => ClientManager.SystemConfiguration.AutomationConfiguration.Procedures);
		}

		#region IPlanExtension Members

		public override int Index
		{
			get { return 1; }
		}
		public override string Title
		{
			get { return "Автоматизация"; }
		}

		public override IEnumerable<IInstrument> Instruments
		{
			get
			{
				if (_instruments == null)
					_instruments = new List<IInstrument>()
					{
						new InstrumentViewModel()
						{
							ImageSource="Procedure",
							ToolTip="Процедура",
							Adorner = new ProcedureRectangleAdorner(DesignerCanvas, _proceduresViewModel),
							Index = 400,
							Autostart = true
						},
					};
				return _instruments;
			}
		}

		public override bool ElementAdded(Plan plan, ElementBase element)
		{
			if (element is ElementProcedure)
			{
				var elementProcedure = (ElementProcedure)element;
				plan.ElementExtensions.Add(elementProcedure);
				SetItem<Procedure>(elementProcedure);
				return true;
			}
			return false;
		}
		public override bool ElementRemoved(Plan plan, ElementBase element)
		{
			if (element is ElementProcedure)
			{
				var elementProcedure = (ElementProcedure)element;
				plan.ElementExtensions.Remove(elementProcedure);
				ResetItem<Procedure>(elementProcedure);
				return true;
			}
			return false;
		}

		public override void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementProcedure)
				RegisterDesignerItem<Procedure>(designerItem, "Procedure", "/Controls;component/Images/ProcedureYellow.png");
		}

		public override IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			if (plan.ElementExtensions == null)
				plan.ElementExtensions = new List<ElementBase>();
			foreach (var element in plan.ElementExtensions)
				if (element is ElementProcedure)
					yield return element;
		}

		public override void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			base.ExtensionRegistered(designerCanvas);
			LayerGroupService.Instance.RegisterGroup("Procedure", "Процедуры", 42);
		}
		public override void ExtensionAttached()
		{
			using (new TimeCounter("Automation.ExtensionAttached.BuildMap: {0}"))
				base.ExtensionAttached();
		}

		public override IEnumerable<ElementError> Validate()
		{
			List<ElementError> errors = new List<ElementError>();
			if (!GlobalSettingsHelper.GlobalSettings.IgnoredErrors.Contains(ValidationErrorType.NotBoundedElements))
				ClientManager.PlansConfiguration.AllPlans.ForEach(plan =>
					errors.AddRange(FindUnbindedErrors<ElementProcedure, ShowProceduresEvent, Guid>(plan.ElementExtensions.OfType<ElementProcedure>(), plan.UID, "Несвязанная процедура", "/Controls;component/Images/ProcedureYellow.png", Guid.Empty)));
			return errors;
		}
		#endregion

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			ElementProcedure element = e.Element as ElementProcedure;
			if (element != null)
				e.PropertyViewModel = new ProcedurePropertiesViewModel(element, _proceduresViewModel);
		}

		protected override void UpdateDesignerItemProperties<TItem>(CommonDesignerItem designerItem, TItem item)
		{
			if (typeof(TItem) == typeof(Procedure))
			{
				var procedure = item as Procedure;
				designerItem.Title = procedure == null ? "Несвязанная процедура" : procedure.Name;
			}
		}
	}
}