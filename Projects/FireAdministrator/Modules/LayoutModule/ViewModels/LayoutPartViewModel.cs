using System;
using System.Linq;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows.ViewModels;

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
		}
		public LayoutPartViewModel(LayoutPartDescriptionViewModel layoutPartDescriptionViewModel)
		{
			LayoutPartDescriptionViewModel = layoutPartDescriptionViewModel;
			LayoutPart = new LayoutPart()
			{
				DescriptionUID = LayoutPartDescriptionViewModel.LayoutPartDescription.UID,
				UID = Guid.NewGuid(),
			};
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
			get { return LayoutPartDescriptionViewModel.Content; }
		}
	}
}
