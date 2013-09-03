using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Services;
using Infrastructure.Client.Plans;
using PlansModule.Kursk.ViewModels;
using PlansModule.Kursk.InstrumentAdorners;

namespace PlansModule.Kursk
{
	public class PlanExtension : IPlanExtension<Plan>
	{
		private const string OthersGroup = "Others";
		private TanksViewModel _tanksViewModel;
		private CommonDesignerCanvas _designerCanvas;
		private IEnumerable<IInstrument> _instruments;

		public PlanExtension(TanksViewModel tanksViewModel)
		{
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			_instruments = null;
			_tanksViewModel = tanksViewModel;
		}

		public void Initialize()
		{
		}

		#region IPlanExtension Members

		public string Title
		{
			get { return "Устройства - Курск"; }
		}

		public IEnumerable<IInstrument> Instruments
		{
			get
			{
				if (_instruments == null)
					_instruments = new List<IInstrument>()
				    {
						new InstrumentViewModel()
						{
						    ImageSource="/Controls;component/Images/Tank.png",
						    ToolTip="Бак",
						    Adorner = new TankRectangleAdorner(_designerCanvas, _tanksViewModel),
						    Index = 299,
						    Autostart = true
						},
				    };
				return _instruments;
			}
		}

		public bool ElementAdded(Plan plan, ElementBase element)
		{
			var elementRectangleTank = element as ElementRectangleTank;
			if (elementRectangleTank != null)
			{
				plan.ElementExtensions.Add(element);
				return true;
			}
			return false;
		}
		public bool ElementRemoved(Plan plan, ElementBase element)
		{
			var elementRectangleTank = element as ElementRectangleTank;
			if (elementRectangleTank != null)
			{
				plan.ElementExtensions.Remove(element);
				return true;
			}
			return false;
		}

		public void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementRectangleTank)
			{
				designerItem.Group = OthersGroup;
				designerItem.Title = "Бак";
				//designerItem.ItemPropertyChanged += new EventHandler(DevicePropertyChanged);
				//designerItem.UpdateProperties += UpdateDesignerItemDevice;
			}
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			foreach (var element in plan.ElementExtensions)
				if (element is ElementRectangleTank)
					yield return element;
		}

		public void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			_designerCanvas = designerCanvas;
			LayerGroupService.Instance.RegisterGroup(OthersGroup, "Прочие", 4);
		}
		public void ExtensionAttached()
		{
		}

		#endregion

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			//var elementXDevice = args.Element as ElementXDevice;
			//if (elementXDevice != null)
			//    args.Painter = new Painter(elementXDevice);
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			var elementRectangleTank = e.Element as ElementRectangleTank;
			if (elementRectangleTank != null)
				e.PropertyViewModel = new TankPropertiesViewModel(elementRectangleTank, _tanksViewModel);
		}
	}
}
