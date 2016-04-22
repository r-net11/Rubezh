using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Practices.Unity.Utility;
using RubezhAPI.Models.Layouts;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
			LayoutPartDescriptionViewModel = LayoutDesignerViewModel.Instance.LayoutElementsViewModel.GetLayoutPartDescription(LayoutPart.DescriptionUID) ?? new LayoutPartDescriptionViewModel(new UnknownLayoutPartDescription(LayoutPart.DescriptionUID));
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
			LayoutPart.Properties = Content.Properties;
		}

		void Initialize()
		{
			Content = LayoutPartDescriptionViewModel.LayoutPartDescription.CreateContent((ILayoutProperties)LayoutPart.Properties) ?? new LayoutPartTitleViewModel() { Title = Title, IconSource = IconSource };
			ConfigureCommand = new RelayCommand(OnConfigureCommand, CanConfigureCommand);
			UpdateTitle();
		}

		public Guid UID
		{
			get { return LayoutPart.UID; }
		}
		public string Title
		{
			get { return LayoutPart.Title ?? LayoutPartDescriptionViewModel.Name; }
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
		bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				OnPropertyChanged(() => IsSelected);
			}
		}

		bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				_isActive = value;
				OnPropertyChanged(() => IsActive);
			}
		}


		public RelayCommand ConfigureCommand { get; private set; }

		void OnConfigureCommand()
		{
			var layoutPartPropertiesViewModel = new LayoutPartPropertiesViewModel(this);
			if (DialogService.ShowModalWindow(layoutPartPropertiesViewModel))
				UpdateSize(layoutPartPropertiesViewModel.LayoutSize);
		}

		bool CanConfigureCommand()
		{
			return true;
		}

		public LayoutPartSize GetSize()
		{
			var document = GetLayoutDocument();
			var pair = GetLayoutPositionableElements(document);
			var layoutItem = LayoutDesignerViewModel.Instance.Manager.GetLayoutItemFromModel(document);
			var size = new LayoutPartSize();
			size.PreferedSize = LayoutPartDescriptionViewModel.LayoutPartDescription.Size.PreferedSize;
			size.Margin = document.Margin;
			size.BackgroundColor = document.BackgroundColor;
			size.BorderColor = document.BorderColor;
			size.BorderThickness = document.BorderThickness;
			ReadSize(size, pair.First, layoutItem);
			ReadSize(size, pair.Second, layoutItem);
			ValidateSize(size);
			return size;
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
			document.BackgroundColor = layoutPartSize.BackgroundColor;
			document.BorderColor = layoutPartSize.BorderColor;
			document.BorderThickness = layoutPartSize.BorderThickness;
			UpdateTitle();
		}

		void UpdateTitle()
		{
			var layoutPartTitleViewModel = Content as LayoutPartTitleViewModel;
			if (layoutPartTitleViewModel != null)
			{
				layoutPartTitleViewModel.Title = Title;
				if (string.IsNullOrEmpty(layoutPartTitleViewModel.IconSource))
					layoutPartTitleViewModel.IconSource = IconSource;
			}
			OnPropertyChanged(() => Title);
		}

		LayoutDocument GetLayoutDocument()
		{
			var manager = LayoutDesignerViewModel.Instance.Manager;
			return manager.Layout.Descendents().OfType<LayoutDocument>().FirstOrDefault(item => item.Content == this);
		}

		Pair<ILayoutPositionableElement, ILayoutPositionableElement> GetLayoutPositionableElements(LayoutDocument layoutDocument)
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

		void ReadSize(LayoutPartSize size, ILayoutPositionableElement element, LayoutItem layoutItem)
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
						size.Width = element.DockWidth.Value;
						break;
					case Orientation.Vertical:
						size.MinHeight = element.DockMinHeight;
						size.IsHeightFixed = element.IsDockHeightFixed;
						size.HeightType = element.DockHeight.GridUnitType;
						size.Height = element.DockHeight.Value;
						break;
				}
			}
		}

		void WriteSize(LayoutPartSize size, ILayoutPositionableElement element, LayoutItem layoutItem)
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
						break;
					case Orientation.Vertical:
						element.DockMinHeight = size.MinHeight;
						element.IsDockHeightFixed = size.IsHeightFixed;
						element.DockHeight = new GridLength(size.Height, size.HeightType);
						break;
				}
			}
		}

		void ValidateSize(LayoutPartSize size)
		{
			if (double.IsNaN(size.Width))
				size.Width = LayoutPartDescriptionViewModel.LayoutPartDescription.Size.Width;
			if (double.IsNaN(size.Height))
				size.Height = LayoutPartDescriptionViewModel.LayoutPartDescription.Size.Height;
			if (size.HeightType == GridUnitType.Auto && size.Height == 0)
				size.Height = 1;
			if (size.WidthType == GridUnitType.Auto && size.Width == 0)
				size.Width = 1;
		}
	}
}