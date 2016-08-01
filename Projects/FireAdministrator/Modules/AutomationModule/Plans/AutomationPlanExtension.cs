using AutomationModule.Events;
using AutomationModule.Plans.InstrumentAdorners;
using AutomationModule.Plans.ViewModels;
using AutomationModule.ViewModels;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Services;
using StrazhAPI.Automation;
using StrazhAPI.Models;
using StrazhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using Localization.Automation.Common;
using Localization.Automation.Errors;

namespace AutomationModule.Plans
{
	class AutomationPlanExtension : BasePlanExtension
	{
		public static AutomationPlanExtension Instance { get; private set; }

		private readonly ProceduresViewModel _proceduresViewModel;
		private IEnumerable<IInstrument> _instruments;

		public AutomationPlanExtension(ProceduresViewModel proceduresViewModel)
		{
			Instance = this;
			ServiceFactoryBase.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactoryBase.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactoryBase.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactoryBase.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			_proceduresViewModel = proceduresViewModel;
			_instruments = null;
			Cache.Add(() => FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures);
		}

		#region IPlanExtension Members

		public override int Index
		{
			get { return 1; }
		}
		public override string Title
		{
            get { return CommonResources.Automation; }
		}

		public override IEnumerable<IInstrument> Instruments
		{
			get
			{
				if (_instruments != null) return _instruments;

				_instruments = new List<IInstrument>();

				// В редакторе планов кнопка "Процедура" доступна, если этого не запрещает лицензия
				if (ServiceFactory.UiElementsVisibilityService.IsMainMenuAutomationElementVisible)
				{
					((List<IInstrument>)_instruments).Add(new InstrumentViewModel
					{
						ImageSource = "Procedure",
						ToolTip = "CommonResources.Procedure",
						Adorner = new ProcedureRectangleAdorner(DesignerCanvas, _proceduresViewModel),
						Index = 400,
						Autostart = true
					});
				}
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

			return plan.ElementExtensions.OfType<ElementProcedure>();
		}

		public override void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			base.ExtensionRegistered(designerCanvas);
			LayerGroupService.Instance.RegisterGroup("Procedure", CommonResources.Procedures, 42);
		}

		public override IEnumerable<ElementError> Validate()
		{
			var errors = new List<ElementError>();
			FiresecManager.PlansConfiguration.AllPlans.ForEach(plan =>
				errors.AddRange(FindUnbindedErrors<ElementProcedure, ShowProceduresEvent, Guid>(plan.ElementExtensions.OfType<ElementProcedure>(), plan.UID, CommonErrors.UnboundProcedure_Error, "/Controls;component/Images/ProcedureYellow.png", Guid.Empty)));

			return errors;
		}
		#endregion

		private static void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			var element = e.Element as ElementProcedure;

			if (element != null)
				e.PropertyViewModel = new ProcedurePropertiesViewModel(element, _proceduresViewModel);
		}

		protected override void UpdateDesignerItemProperties<TItem>(CommonDesignerItem designerItem, TItem item)
		{
			if (typeof (TItem) != typeof (Procedure)) return;

			var procedure = item as Procedure;
			designerItem.Title = procedure == null ? CommonErrors.UnboundProcedure_Error : procedure.Name;
		}
	}
}