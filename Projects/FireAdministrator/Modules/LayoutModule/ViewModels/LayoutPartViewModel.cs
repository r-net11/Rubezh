using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FiresecAPI.Models.Layouts;
using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Practices.Unity.Utility;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace LayoutModule.ViewModels
{
	public class LayoutPartViewModel : BaseViewModel
	{
		public LayoutPartDescriptionViewModel LayoutPartDescriptionViewModel { get; private set; }
		public LayoutPart LayoutPart { get; private set; }

		public LayoutPartViewModel(LayoutPart layoutPart)
		{
			LayoutPart = layoutPart;
			LayoutPartDescriptionViewModel = LayoutDesignerViewModel.Instance.LayoutElementsViewModel.LayoutParts.FirstOrDefault(item => item.LayoutPartDescription.UID == LayoutPart.DescriptionUID) ?? new LayoutPartDescriptionViewModel(new UnknownLayoutPartDescription(LayoutPart.DescriptionUID));
			Initialize();
		}
		public LayoutPartViewModel(LayoutPartDescriptionViewModel layoutPartDescriptionViewModel)
		{
			LayoutPartDescriptionViewModel = layoutPartDescriptionViewModel;
			LayoutPart = new LayoutPart()
			{
				DescriptionUID = LayoutPartDescriptionViewModel.LayoutPartDescription.UID,
				UID = Guid.NewGuid(),
			};
			Initialize();
			LayoutPart.Properties = Content.GetProperties();
		}
		private void Initialize()
		{
			Content = LayoutPartDescriptionViewModel.Content ?? new LayoutPartTitleViewModel() { Title = Title, IconSource = IconSource };
			Content.SetProperties(LayoutPart.Properties);
			ConfigureCommand = new RelayCommand(OnConfigureCommand, CanConfigureCommand);
		}

		public Guid UID
		{
			get { return LayoutPart.UID; }
		}
		public string Title
		{
			get { return LayoutPartDescriptionViewModel.Name; }
		}
		public string IconSource
		{
			get { return LayoutPartDescriptionViewModel.IconSource; }
		}
		public string Description
		{
			get { return LayoutPartDescriptionViewModel.Description; }
		}
		public BaseLayoutPartViewModel Content { get; private set; }

		public RelayCommand ConfigureCommand { get; private set; }
		private void OnConfigureCommand()
		{
			var document = GetLayoutDocument();
			var pair = GetLayoutPositionableElements(document);
			var layoutItem = LayoutDesignerViewModel.Instance.Manager.GetLayoutItemFromModel(document);
			var size = new LayoutPartSize();
			size.PreferedSize = LayoutPartDescriptionViewModel.LayoutPartDescription.PreferedSize;
			size.Margin = (int)document.Margin;
			ReadSize(size, pair.First, layoutItem);
			ReadSize(size, pair.Second, layoutItem);
			ValidateSize(size);
			var layoutPartPropertiesViewModel = new LayoutPartPropertiesViewModel(size);
			if (DialogService.ShowModalWindow(layoutPartPropertiesViewModel))
			{
				ValidateSize(size);
				WriteSize(size, pair.First, layoutItem);
				WriteSize(size, pair.Second, layoutItem);
				document.Margin = size.Margin;
			}
		}
		private bool CanConfigureCommand()
		{
			return true;
		}

		public void UpdateSize(LayoutPartSize layoutPartSize)
		{
			ValidateSize(layoutPartSize);
			var document = GetLayoutDocument();
			var pair = GetLayoutPositionableElements(document);
			var layoutItem = LayoutDesignerViewModel.Instance.Manager.GetLayoutItemFromModel(document);
			WriteSize(layoutPartSize, pair.First, layoutItem);
			WriteSize(layoutPartSize, pair.Second, layoutItem);
			document.Margin = layoutPartSize.Margin;
		}
		private LayoutDocument GetLayoutDocument()
		{
			var manager = LayoutDesignerViewModel.Instance.Manager;
			return manager.Layout.Descendents().OfType<LayoutDocument>().FirstOrDefault(item => item.Content == this);
		}
		private Pair<ILayoutPositionableElement, ILayoutPositionableElement> GetLayoutPositionableElements(LayoutDocument layoutDocument)
		{
			var layoutDocumentPane = (ILayoutPositionableElement)layoutDocument.Parent;
			ILayoutOrientableGroup container = (ILayoutOrientableGroup)layoutDocumentPane.Parent;
			var orientation = container.Orientation;
			if (container.Parent == container.Root)
				container = null;
			else
				while (((ILayoutOrientableGroup)container.Parent).Orientation == orientation)
				{
					container = (ILayoutOrientableGroup)container.Parent;
					if (container.Parent == container.Root)
					{
						container = null;
						break;
					}
				}
			return new Pair<ILayoutPositionableElement, ILayoutPositionableElement>(layoutDocumentPane, (ILayoutPositionableElement)container);
		}
		private void ReadSize(LayoutPartSize size, ILayoutPositionableElement element, LayoutItem layoutItem)
		{
			if (element != null)
			{
				var container = (ILayoutOrientableGroup)element.Parent;
				switch (container.Orientation)
				{
					case Orientation.Horizontal:
						size.MinWidth = element.DockMinWidth;
						size.IsWidthFixed = element.IsDockWidthFixed;
						size.WidthType = element.DockWidth.GridUnitType;
						size.Width = element.DockWidth.IsAuto ? layoutItem.View.Width : element.DockWidth.Value;
						break;
					case Orientation.Vertical:
						size.MinHeight = element.DockMinHeight;
						size.IsHeightFixed = element.IsDockHeightFixed;
						size.HeightType = element.DockHeight.GridUnitType;
						size.Height = element.DockHeight.IsAuto ? layoutItem.View.Height : element.DockHeight.Value;
						break;
				}
			}
		}
		private void WriteSize(LayoutPartSize size, ILayoutPositionableElement element, LayoutItem layoutItem)
		{
			if (element != null)
			{
				var container = (ILayoutOrientableGroup)element.Parent;
				switch (container.Orientation)
				{
					case Orientation.Horizontal:
						element.DockMinWidth = size.MinWidth;
						element.IsDockWidthFixed = size.IsWidthFixed;
						element.DockWidth = new GridLength(size.Width, size.WidthType);
						layoutItem.View.Width = size.WidthType == GridUnitType.Auto ? size.Width : double.NaN;
						break;
					case Orientation.Vertical:
						element.DockMinHeight = size.MinHeight;
						element.IsDockHeightFixed = size.IsHeightFixed;
						element.DockHeight = new GridLength(size.Height, size.HeightType);
						layoutItem.View.Height = size.HeightType == GridUnitType.Auto ? size.Height : double.NaN;
						break;
				}
			}
		}
		private void ValidateSize(LayoutPartSize size)
		{
			if (double.IsNaN(size.Width))
				size.Width = LayoutPartDescriptionViewModel.LayoutPartDescription.PreferedSize.Width;
			if (double.IsNaN(size.Height))
				size.Height = LayoutPartDescriptionViewModel.LayoutPartDescription.PreferedSize.Height;
		}
	}
}
