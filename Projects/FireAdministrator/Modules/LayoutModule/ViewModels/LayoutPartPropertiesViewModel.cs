using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows.ViewModels;
using Localization.Layout.ViewModels;

namespace LayoutModule.ViewModels
{
	public class LayoutPartPropertiesViewModel : SaveCancelDialogViewModel
	{
		public LayoutPartSize LayoutSize { get; private set; }
		public LayoutPartViewModel LayoutPartViewModel { get; private set; }
		public LayoutPartPropertiesViewModel(LayoutPartViewModel layoutPartViewModel)
		{
			Title = CommonViewModels.LayoutPartProperties_Title;
			LayoutPartViewModel = layoutPartViewModel;
			LayoutSize = LayoutPartViewModel.GetSize();
			PropertyPages = new ObservableCollection<LayoutPartPropertyPageViewModel>();
			PropertyPages.Add(new LayoutPartPropertyGeneralPageViewModel(LayoutPartViewModel, LayoutSize));
			foreach (var page in LayoutPartViewModel.Content.PropertyPages)
				PropertyPages.Add(page);
			CopyProperties();
		}

		private void CopyProperties()
		{
			foreach (var page in PropertyPages)
				page.CopyProperties();
		}

		public ObservableCollection<LayoutPartPropertyPageViewModel> PropertyPages { get; private set; }

		protected override bool CanSave()
		{
			return PropertyPages.All(page => page.CanSave());
		}

		protected override bool Save()
		{
			var haveChanges = false;
			foreach (var page in PropertyPages)
				if (page.Save())
					haveChanges = true;
			if (haveChanges)
				LayoutDesignerViewModel.Instance.LayoutConfigurationChanged();
			return base.Save();
		}
	}
}