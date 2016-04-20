using Infrastructure;
using Infrastructure.Common.Windows.Services.Layout;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using LayoutModel = RubezhAPI.Models.Layouts.Layout;

namespace FireMonitor.Layout.ViewModels
{
	public class LayoutContainer
	{
		XmlLayoutSerializer _serializer;
		BaseViewModel _viewModel;
		public event EventHandler LayoutChanging;
		public event EventHandler LayoutChanged;
		public LayoutModel Layout { get; private set; }

		public LayoutContainer(BaseViewModel viewModel, LayoutModel layout = null)
		{
			_viewModel = viewModel;
			Layout = layout;
		}

		public void UpdateLayout(LayoutModel layout)
		{
			Layout = layout;
			if (LayoutChanging != null)
				LayoutChanging(this, EventArgs.Empty);
			if (Layout != null && Manager != null)
			{
				Initialize();
				Manager.GridSplitterHeight = Layout.SplitterSize;
				Manager.GridSplitterWidth = Layout.SplitterSize;
				Manager.GridSplitterBackground = new SolidColorBrush(Layout.SplitterColor.ToWindowsColor());
				Manager.BorderBrush = new SolidColorBrush(Layout.BorderColor.ToWindowsColor());
				Manager.BorderThickness = new Thickness(Layout.BorderThickness);
				if (_serializer != null && Layout != null && !string.IsNullOrEmpty(Layout.Content))
					using (var tr = new StringReader(Layout.Content))
						_serializer.Deserialize(tr);
				LoadLayoutProperties();
			}
			if (LayoutChanged != null)
				LayoutChanged(this, EventArgs.Empty);
		}
		public void Initialize()
		{
			var list = new List<LayoutPartViewModel>();
			var map = new Dictionary<Guid, ILayoutPartPresenter>();
			foreach (var module in ApplicationService.Modules)
			{
				var layoutProviderModule = module as ILayoutProviderModule;
				if (layoutProviderModule != null)
					foreach (var layoutPart in layoutProviderModule.GetLayoutParts())
						map.Add(layoutPart.UID, layoutPart);
			}
			if (Layout != null)
				foreach (var layoutPart in Layout.Parts)
					list.Add(new LayoutPartViewModel(layoutPart, map.ContainsKey(layoutPart.DescriptionUID) ? map[layoutPart.DescriptionUID] : new UnknownLayoutPartPresenter(layoutPart.DescriptionUID)));
			LayoutParts = new ObservableCollection<LayoutPartViewModel>(list);
		}
		private void LoadLayoutProperties()
		{
			var properties = ClientManager.FiresecService.GetProperties(Layout.UID);
			if (properties != null)
			{
				if (properties.VisualProperties != null)
					foreach (var visualProperty in properties.VisualProperties)
					{
						var layoutPart = LayoutParts.FirstOrDefault(item => item.UID == visualProperty.LayoutPart);
						if (layoutPart != null)
							layoutPart.SetProperty(visualProperty.Property, visualProperty.Value);
					}
				if (properties.PlanProperties != null)
					ServiceFactory.Events.GetEvent<ChangePlanPropertiesEvent>().Publish(properties.PlanProperties);
			}
		}

		ObservableCollection<LayoutPartViewModel> _layoutParts;
		public ObservableCollection<LayoutPartViewModel> LayoutParts
		{
			get { return _layoutParts; }
			set
			{
				if (_layoutParts != null)
					_layoutParts.CollectionChanged -= LayoutPartsChanged;
				_layoutParts = value;
				_layoutParts.CollectionChanged += LayoutPartsChanged;
				_viewModel.OnPropertyChanged(() => LayoutParts);
			}
		}

		LayoutPartViewModel _activeLayoutPart;
		public LayoutPartViewModel ActiveLayoutPart
		{
			get { return _activeLayoutPart; }
			set
			{
				_activeLayoutPart = value;
				_viewModel.OnPropertyChanged(() => ActiveLayoutPart);
			}
		}

		DockingManager _manager;
		public DockingManager Manager
		{
			get { return _manager; }
			set
			{
				if (Manager != null)
				{
					Manager.DocumentClosing -= ManagerLayoutPartClosing;
					Manager.LayoutChanged -= ManagerLayoutChanged;
				}
				_manager = value;
				if (_serializer != null)
				{
					_serializer.LayoutSerializationCallback -= LayoutSerializationCallback;
					_serializer = null;
				}
				if (_manager != null)
				{
					Manager.LayoutChanged += ManagerLayoutChanged;
					Manager.DocumentClosing += ManagerLayoutPartClosing;
					Manager.LayoutUpdateStrategy = new LayoutUpdateStrategy();
					_serializer = new XmlLayoutSerializer(Manager);
					_serializer.LayoutSerializationCallback += LayoutSerializationCallback;
					UpdateLayout(Layout);
				}
			}
		}

		private void LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(e.Model.ContentId))
			{
				var layoutPart = LayoutParts.FirstOrDefault(item => item.UID == Guid.Parse(e.Model.ContentId));
				if (layoutPart != null)
				{
					layoutPart.Margin = e.Model.Margin;
					layoutPart.BorderColor = e.Model.BorderColor.ToRubezhColor();
					layoutPart.BorderThickness = e.Model.BorderThickness;
					layoutPart.BackgroundColor = e.Model.BackgroundColor.ToRubezhColor();
				}
				e.Content = layoutPart;
			}
		}
		private void ManagerLayoutChanged(object sender, EventArgs e)
		{
		}
		private void ManagerLayoutPartClosing(object sender, DocumentClosingEventArgs e)
		{
			var layoutPartViewModel = e.Document.Content as LayoutPartViewModel;
			if (layoutPartViewModel != null)
			{
				LayoutParts.Remove(layoutPartViewModel);
				e.Cancel = true;
			}
		}
		private void LayoutPartsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
		}
	}
}