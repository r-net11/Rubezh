﻿using System.Collections.ObjectModel;
using Infrastructure.Common;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows.ViewModels;

namespace LayoutModule.ViewModels
{
	public class LayoutPartPropertiesViewModel : SaveCancelDialogViewModel
	{
		public LayoutPartSize LayoutSize { get; private set; }
		public LayoutPartViewModel LayoutPartViewModel { get; private set; }
		public LayoutPartPropertiesViewModel(LayoutPartViewModel layoutPartViewModel)
		{
			Title = "Свойства элемента";
			LayoutPartViewModel = layoutPartViewModel;
			LayoutSize = LayoutPartViewModel.GetSize();
			using (new WaitWrapper())
			{
				PropertyPages = new ObservableCollection<LayoutPartPropertyPageViewModel>();
				PropertyPages.Add(new LayoutPartPropertyGeneralPageViewModel(LayoutPartViewModel, LayoutSize));
				foreach (var page in LayoutPartViewModel.Content.PropertyPages)
					PropertyPages.Add(page);
				CopyProperties();
			}
		}

		private void CopyProperties()
		{
			foreach (var page in PropertyPages)
				page.CopyProperties();
		}

		public ObservableCollection<LayoutPartPropertyPageViewModel> PropertyPages { get; private set; }

		protected override bool CanSave()
		{
			using (new WaitWrapper())
				foreach (var page in PropertyPages)
					if (!page.CanSave())
						return false;
			return true;
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