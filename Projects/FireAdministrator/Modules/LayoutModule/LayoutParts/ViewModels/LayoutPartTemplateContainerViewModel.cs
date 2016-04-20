using Infrastructure.Common.Services.Layout;
using Infrastructure.Plans;
using LayoutModule.ViewModels;
using RubezhAPI.Models.Layouts;
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

namespace LayoutModule.LayoutParts.ViewModels
{
	public class LayoutPartTemplateContainerViewModel : BaseLayoutPartViewModel
	{
		private LayoutPartReferenceProperties _properties;
		private LayoutPartPropertyTemplateContainerPageViewModel _containerPage;
		public LayoutPartTemplateContainerViewModel(LayoutPartReferenceProperties properties)
		{
			_properties = properties ?? new LayoutPartReferenceProperties();
			_containerPage = new LayoutPartPropertyTemplateContainerPageViewModel(this);
			Layout = MonitorLayoutsViewModel.Instance.Layouts.Where(item => item.Layout.UID == _properties.ReferenceUID).Select(item => item.Layout).FirstOrDefault();
			LoadLayout();
		}

		public override ILayoutProperties Properties
		{
			get { return _properties; }
		}
		public override IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages
		{
			get
			{
				yield return _containerPage;
			}
		}

		public LayoutModel Layout { get; private set; }
		private XmlLayoutSerializer _serializer;

		public void UpdateLayout(LayoutModel layout)
		{
			Layout = layout;
			Initialize();
			if (Layout != null && Manager != null)
			{
				Manager.GridSplitterHeight = Layout.SplitterSize;
				Manager.GridSplitterWidth = Layout.SplitterSize;
				Manager.GridSplitterBackground = new SolidColorBrush(Layout.SplitterColor.ToWindowsColor());
				Manager.BorderBrush = new SolidColorBrush(Layout.BorderColor.ToWindowsColor());
				Manager.BorderThickness = new Thickness(Layout.BorderThickness);
				if (_serializer != null && !string.IsNullOrEmpty(Layout.Content))
					using (var tr = new StringReader(Layout.Content))
						_serializer.Deserialize(tr);
			}
		}
		private void LoadLayout()
		{
			UpdateLayout(Layout);
		}
		private void Initialize()
		{
			var list = new List<LayoutPartViewModel>();
			if (Layout != null)
				foreach (var layoutPart in Layout.Parts)
					list.Add(new LayoutPartViewModel(layoutPart));
			LayoutParts = new ObservableCollection<LayoutPartViewModel>(list);
		}

		private ObservableCollection<LayoutPartViewModel> _layoutParts;
		public ObservableCollection<LayoutPartViewModel> LayoutParts
		{
			get { return _layoutParts; }
			set
			{
				if (_layoutParts != null)
					_layoutParts.CollectionChanged -= LayoutPartsChanged;
				_layoutParts = value;
				_layoutParts.CollectionChanged += LayoutPartsChanged;
				OnPropertyChanged(() => LayoutParts);
			}
		}

		private LayoutPartViewModel _activeLayoutPart;
		public LayoutPartViewModel ActiveLayoutPart
		{
			get { return _activeLayoutPart; }
			set
			{
				_activeLayoutPart = value;
				OnPropertyChanged(() => ActiveLayoutPart);
			}
		}

		private DockingManager _manager;
		public DockingManager Manager
		{
			get { return _manager; }
			set
			{
				if (Manager != null)
				{
					Manager.DocumentClosing -= LayoutPartClosing;
					Manager.LayoutChanged -= LayoutChanged;
				}
				_manager = value;
				if (_serializer != null)
				{
					_serializer.LayoutSerializationCallback -= LayoutSerializationCallback;
					_serializer = null;
				}
				if (_manager != null)
				{
					Manager.LayoutChanged += LayoutChanged;
					Manager.DocumentClosing += LayoutPartClosing;
					Manager.LayoutUpdateStrategy = new LayoutUpdateStrategy();
					_serializer = new XmlLayoutSerializer(Manager);
					_serializer.LayoutSerializationCallback += LayoutSerializationCallback;
					LoadLayout();
				}
			}
		}

		private void LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(e.Model.ContentId))
				e.Content = LayoutParts.FirstOrDefault(item => item.UID == Guid.Parse(e.Model.ContentId));
		}
		private void LayoutChanged(object sender, EventArgs e)
		{
		}
		private void LayoutPartClosing(object sender, DocumentClosingEventArgs e)
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
