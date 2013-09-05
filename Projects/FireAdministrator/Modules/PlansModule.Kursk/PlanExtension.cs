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
using XFiresecAPI;
using PlansModule.Kursk.Designer;

namespace PlansModule.Kursk
{
	public class PlanExtension : IPlanExtension<Plan>
	{
		private const string OthersGroup = "Others";
		private CommonDesignerCanvas _designerCanvas;
		private IEnumerable<IInstrument> _instruments;

		public PlanExtension()
		{
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			_instruments = null;
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
						    Adorner = new TankRectangleAdorner(_designerCanvas),
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
				Helper.SetXDevice(elementRectangleTank);
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
			var elementRectangleTank = designerItem.Element as ElementRectangleTank;
			if (elementRectangleTank != null)
			{
				designerItem.Group = OthersGroup;
				designerItem.UpdateProperties += UpdateDesignerItemXDevice;
				UpdateDesignerItemXDevice(designerItem);
				OnXDevicePropertyChanged(designerItem);
				designerItem.ItemPropertyChanged += new EventHandler(XDevicePropertyChanged);
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
				e.PropertyViewModel = new TankPropertiesViewModel(elementRectangleTank);
		}

		private void UpdateDesignerItemXDevice(CommonDesignerItem designerItem)
		{
			var elementRectangleTank = designerItem.Element as ElementRectangleTank;
			var xdevice = Helper.GetXDevice(elementRectangleTank);
			Helper.SetXDevice(elementRectangleTank, xdevice);
			elementRectangleTank.BackgroundColor = Helper.GetTankColor(xdevice);
			designerItem.Title = Helper.GetTankTitle(elementRectangleTank);
		}
		private void XDevicePropertyChanged(object sender, EventArgs e)
		{
			DesignerItem designerItem = (DesignerItem)sender;
			OnXDevicePropertyChanged(designerItem);
		}
		private void OnXDevicePropertyChanged(DesignerItem designerItem)
		{
			var device = Helper.GetXDevice((ElementRectangleTank)designerItem.Element);
			if (device != null)
				device.Changed += () =>
				{
					UpdateDesignerItemXDevice(designerItem);
					designerItem.Painter.Invalidate();
					_designerCanvas.Refresh();
				};
		}
	}
}
