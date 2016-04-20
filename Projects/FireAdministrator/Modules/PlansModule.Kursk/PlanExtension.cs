using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Events;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Services;
using PlansModule.Kursk.Designer;
using PlansModule.Kursk.InstrumentAdorners;
using PlansModule.Kursk.ViewModels;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlansModule.Kursk
{
	public class PlanExtension : BasePlanExtension
	{
		private const string OthersGroup = "Others";
		public static PlanExtension Instance { get; private set; }

		private IEnumerable<IInstrument> _instruments;

		public PlanExtension()
		{
			Instance = this;
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			_instruments = null;
			Cache.Add<GKDevice>();
		}

		#region IPlanExtension Members

		public override int Index
		{
			get { return 1; }
		}
		public override string Title
		{
			get { return "Устройства - Курск"; }
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
							ImageSource="Tank",
							ToolTip="Бак",
							Adorner = new TankRectangleAdorner(DesignerCanvas),
							Index = 299,
							Autostart = true
						},
					};
				return _instruments;
			}
		}

		public override bool ElementAdded(Plan plan, ElementBase element)
		{
			var elementRectangleTank = element as ElementRectangleTank;
			if (elementRectangleTank != null)
			{
				plan.ElementExtensions.Add(element);
				Helper.SetGKDevice(elementRectangleTank);
				return true;
			}
			return false;
		}
		public override bool ElementRemoved(Plan plan, ElementBase element)
		{
			var elementRectangleTank = element as ElementRectangleTank;
			if (elementRectangleTank != null)
			{
				plan.ElementExtensions.Remove(element);
				return true;
			}
			return false;
		}

		public override void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementRectangleTank)
				RegisterDesignerItem<GKDevice>(designerItem, OthersGroup);
		}

		public override IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			foreach (var element in plan.ElementExtensions)
				if (element is ElementRectangleTank)
					yield return element;
		}

		public override void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			base.ExtensionRegistered(designerCanvas);
			LayerGroupService.Instance.RegisterGroup(OthersGroup, "Прочие", 40);
		}
		public override void ExtensionAttached()
		{
			base.ExtensionAttached();
		}

		public override IEnumerable<ElementError> Validate()
		{
			List<ElementError> errors = new List<ElementError>();
			ClientManager.PlansConfiguration.AllPlans.ForEach(plan =>
				errors.AddRange(FindUnbindedErrors<ElementRectangleTank, ShowGKDeviceEvent, Guid>(plan.ElementExtensions.OfType<ElementRectangleTank>(), plan.UID, "Несвязанный бак", "/Controls;component/Images/BPumpStation.png", Guid.Empty)));
			return errors;
		}

		#endregion

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			var elementRectangleTank = e.Element as ElementRectangleTank;
			if (elementRectangleTank != null)
				e.PropertyViewModel = new TankPropertiesViewModel(elementRectangleTank);
		}

		protected override void UpdateProperties<TItem>(CommonDesignerItem designerItem)
		{
			var elementRectangleTank = designerItem.Element as ElementRectangleTank;
			var device = Helper.GetGKDevice(elementRectangleTank);
			Helper.SetGKDevice(elementRectangleTank, device);
			elementRectangleTank.BackgroundColor = Helper.GetTankColor(device);
			designerItem.Title = Helper.GetTankTitle(elementRectangleTank);
		}
	}
}