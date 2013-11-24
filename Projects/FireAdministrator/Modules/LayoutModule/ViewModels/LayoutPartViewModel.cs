using System;
using System.Linq;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Client.Layout.ViewModels;
using System.Windows;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using System.Windows.Controls;

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
		}
		private void Initialize()
		{
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
		public object Content
		{
			get { return LayoutPartDescriptionViewModel.Content ?? new LayoutPartTitleViewModel() { Title = Title, IconSource = IconSource }; }
		}

		public RelayCommand ConfigureCommand { get; private set; }
		private void OnConfigureCommand()
		{
			var layoutPartPropertiesViewModel = new LayoutPartPropertiesViewModel(GetSize());
			if (DialogService.ShowModalWindow(layoutPartPropertiesViewModel))
				UpdateSize(layoutPartPropertiesViewModel.LayoutPartSize);
		}
		private bool CanConfigureCommand()
		{
			return true;
		}

		public void UpdateSize(LayoutPartSize layoutPartSize)
		{

		}
		private LayoutPartSize GetSize()
		{
			var manager = LayoutDesignerViewModel.Instance.Manager;
			var layoutDocument = manager.Layout.Descendents().OfType<LayoutDocument>().FirstOrDefault(item => item.Content == this);
			var layoutDocumentPane = layoutDocument.Parent as LayoutDocumentPane;
			var container = (LayoutDocumentPaneGroup)layoutDocumentPane.Parent;
			switch (container.Orientation)
			{
				case Orientation.Horizontal:
					break;
				case Orientation.Vertical:
					break;
			}
			return new LayoutPartSize()
			{
			};
		}
	}
}
