using System;
using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Client.Plans;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;
using Infrastructure.Events;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Services;
using Integration.OPC.Plans.InstrumentAdorners;
using Integration.OPC.ViewModels;
using Localization.IntegrationOPC.Errors;
using Localization.IntegrationOPC.ViewModels;
using StrazhAPI;
using StrazhAPI.Integration.OPC;
using StrazhAPI.Models;
using StrazhAPI.Plans.Elements;
using StrazhAPI.Plans.Interfaces;
using StrazhAPI.SKD;

namespace Integration.OPC.Plans
{
	public class OPCPlanExtension : BasePlanExtension
	{
		private IEnumerable<IInstrument> _instruments;
		private readonly ZonesOPCViewModel _zonesViewModel;

		public OPCPlanExtension(ZonesOPCViewModel zonesViewModel)
		{
			//ServiceFactoryBase.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			//ServiceFactoryBase.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactoryBase.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactoryBase.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			_zonesViewModel = zonesViewModel;
			_instruments = null; //TODO: remove it

			Cache.Add(() => SKDManager.SKDConfiguration.OPCZones);
		}

		#region IPlanExtension Members
		public override int Index
		{
			get { return 1; }
		}

		public override string Title
		{
			get { return CommonViewModels.OPCZones; }
		}

		public override bool ElementAdded(Plan plan, ElementBase element)
		{
			if (element is IElementZone)
			{
				if (element is ElementRectangleOPCZone)
					plan.ElementRectangleOPCZones.Add((ElementRectangleOPCZone)element);
				else if (element is ElementPolygonOPCZone)
					plan.ElementPolygonOPCZones.Add((ElementPolygonOPCZone)element);
				else
					return false;
				SetItem<OPCZone>((IElementZone)element);
				return true;
			}
			return false;
		}

		public override bool ElementRemoved(Plan plan, ElementBase element)
		{
			if (element is IElementZone)
			{
				if (element is ElementRectangleOPCZone)
					plan.ElementRectangleOPCZones.Remove((ElementRectangleOPCZone)element);
				else if (element is ElementPolygonOPCZone)
					plan.ElementPolygonOPCZones.Remove((ElementPolygonOPCZone)element);
				else
					return false;
				ResetItem<OPCZone>((IElementZone)element);
				return true;
			}
			return false;
		}

		public override void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementRectangleOPCZone || designerItem.Element is ElementPolygonOPCZone)
				RegisterDesignerItem<OPCZone>(designerItem, "OPCZone", "/Controls;component/Images/SKDZone.png");
		}

		public override IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			if (plan.ElementPolygonOPCZones == null)
				plan.ElementPolygonOPCZones = new List<ElementPolygonOPCZone>();
			if (plan.ElementRectangleOPCZones == null)
				plan.ElementRectangleOPCZones = new List<ElementRectangleOPCZone>();
			foreach (var element in plan.ElementRectangleOPCZones)
				yield return element;
			foreach (var element in plan.ElementPolygonOPCZones)
				yield return element;
		}

		public override void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			base.ExtensionRegistered(designerCanvas);
			LayerGroupService.Instance.RegisterGroup("OPCZone", CommonViewModels.OPCZones, 43);
		}

		public override IEnumerable<IInstrument> Instruments
		{
			get
			{
				return _instruments ??
				       (_instruments = new List<IInstrument>
				    {
						new InstrumentViewModel
						{
							ImageSource = "ZoneRectangle",
							ToolTip = CommonViewModels.OPCZone,
							Adorner = new OPCZoneRectangleAdorner(DesignerCanvas, _zonesViewModel, this),
							Index = 300,
							Autostart = true,
							GroupIndex = 301
						},
						new InstrumentViewModel
						{
							ImageSource = "ZonePolygon",
							ToolTip = CommonViewModels.OPCZone,
							Adorner = new OPCZonePolygonAdorner(DesignerCanvas, _zonesViewModel, this),
							Index = 301,
							Autostart = true,
							GroupIndex = 301
						},
				    });
			}
		}

		public override IEnumerable<ElementError> Validate()
		{
			var errors = new List<ElementError>();
			FiresecManager.PlansConfiguration.AllPlans.ForEach(plan =>
			{
                errors.AddRange(FindUnbindedErrors<ElementRectangleOPCZone, ShowZonesOPCEvent, ShowOnPlanArgs<Guid>>(plan.ElementRectangleOPCZones, plan.UID, CommonViewModels.UnboundZone, "/Controls;component/Images/SKDZone.png", Guid.Empty));
                errors.AddRange(FindUnbindedErrors<ElementPolygonOPCZone, ShowZonesOPCEvent, ShowOnPlanArgs<Guid>>(plan.ElementPolygonOPCZones, plan.UID, CommonViewModels.UnboundZone, "/Controls;component/Images/SKDZone.png", Guid.Empty));
			});
			return errors;
		}
		#endregion

		protected override void UpdateDesignerItemProperties<TItem>(CommonDesignerItem designerItem, TItem item)
		{
			if (typeof(TItem) != typeof(OPCZone)) return;

			var zone = item as OPCZone;
            designerItem.Title = zone == null ? CommonErrors.UnboundZone_Error : zone.Name;
			designerItem.Index = zone == null ? default(int) : zone.No;
		}

		protected override void UpdateElementProperties<TItem>(IElementReference element, TItem item)
		{
			if (typeof(TItem) == typeof(OPCZone))
			{
				var elementZone = (IElementZone)element;
				elementZone.BackgroundColor = GetOPCZoneColor(item as OPCZone);
				elementZone.SetZLayer(item == null ? 50 : 60);
			}
			else
				base.UpdateElementProperties(element, item);
		}

		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			if (e.Element is ElementRectangleOPCZone || e.Element is ElementPolygonOPCZone)
				e.PropertyViewModel = new ZonePropertiesViewModel((IElementZone)e.Element, _zonesViewModel.ZonesOPC, this);
		}

		private static Color GetOPCZoneColor(OPCZone zone)
		{
			var color = Colors.Black;
			if (zone != null)
				color = Colors.Green;
			return color;
		}
	}
}
