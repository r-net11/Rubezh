using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using Controls.Converters;
using FiresecAPI.Models;
using Infrastructure.ViewModels;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
	public partial class PlansViewModel : MenuViewPartViewModel
	{
		private List<IPlanExtension<Plan>> _planExtensions;

		public void RegisterExtension(IPlanExtension<Plan> planExtension)
		{
			if (!_planExtensions.Contains(planExtension))
			{
				_planExtensions.Add(planExtension);
				planExtension.ExtensionRegistered(DesignerCanvas);
				ElementsViewModel.Update();
				DesignerCanvas.Toolbox.RegisterInstruments(planExtension.Instruments);
			}
		}
		public void ElementAdded(ElementBase element)
		{
			foreach (var planExtension in _planExtensions)
				if (planExtension.ElementAdded(SelectedPlan.Plan, element))
					break;
		}
		public void ElementRemoved(ElementBase element)
		{
			foreach (var planExtension in _planExtensions)
				if (planExtension.ElementRemoved(SelectedPlan.Plan, element))
					break;
		}
		public void RegisterDesignerItem(DesignerItem designerItem)
		{
			foreach (var planExtension in _planExtensions)
				planExtension.RegisterDesignerItem(designerItem);
		}
		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			foreach (var planExtension in _planExtensions)
				foreach (var element in planExtension.LoadPlan(plan))
					yield return element;
		}

		private List<TabItem> _tabPages;
		public List<TabItem> TabPages
		{
			get { return _tabPages; }
			set
			{
				_tabPages = value;
				OnPropertyChanged("TabPages");
			}
		}
		private int _selectedTabIndex;
		public int SelectedTabIndex
		{
			get { return _selectedTabIndex; }
			set
			{
				_selectedTabIndex = value;
				OnPropertyChanged("SelectedTabIndex");
			}
		}
		private void CreatePages()
		{
			var layers = new TabItem()
				{
					Header = "Слои",
					Content = ElementsViewModel
				};
			Binding visibilityBinding = new Binding("SelectedPlan");
			visibilityBinding.Source = this;
			visibilityBinding.Converter = new NullToVisibilityConverter();
			layers.SetBinding(TabItem.VisibilityProperty, visibilityBinding);

			TabPages = new List<TabItem>()
			{
				layers
			};
			SelectedTabIndex = -1;
		}
		private void UpdateTabIndex()
		{
			SelectedTabIndex = SelectedPlan == null ? -1 : 0;
		}

		private void ExtensionAttached()
		{
			foreach (var planExtension in _planExtensions)
				planExtension.ExtensionAttached();
		}
	}
}