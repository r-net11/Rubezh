using System;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Events;
using PlansModule.Designer;
using PlansModule.Events;
using Infrustructure.Plans;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using PlansModule.Views;
using System.Windows.Data;
using System.Windows;
using Infrustructure.Plans.Services;

namespace PlansModule.ViewModels
{
	public partial class PlansViewModel : ViewPartViewModel
	{
		public void RegisterExtension(IPlanExtension planExtension)
		{
			if (!string.IsNullOrEmpty(planExtension.Alias))
			{
				LayerGroupService.Instance.RegisterGroup(planExtension.Alias, planExtension.Title, planExtension.Index);
				ElementsViewModel.Update();
			}
			if (planExtension.TabPage != null)
			{
				TabPages.Insert(TabPages.Count - 1, new TabItem()
				{
					Header = planExtension.Title,
					Content = planExtension.TabPage
				});
				OnPropertyChanged("TabPages");
			}
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

		private void CreatePages()
		{
			var layers = new TabItem()
				{
					Header = "Слои",
					Content = ElementsViewModel
				};
			Binding visibilityBinding = new Binding("SelectedPlan");
			visibilityBinding.Source = this;
			visibilityBinding.Converter = (IValueConverter)Application.Current.FindResource("NullToVisibilityConverter");
			layers.SetBinding(TabItem.VisibilityProperty, visibilityBinding);

			TabPages = new List<TabItem>()
			{
				new TabItem()
				{
					Header = "Планы",
					Content = PlansTreeViewModel
				},
				new TabItem()
				{
					Header = "Устройства",
					Content = DevicesViewModel
				},
				layers
			};
		}
	}
}