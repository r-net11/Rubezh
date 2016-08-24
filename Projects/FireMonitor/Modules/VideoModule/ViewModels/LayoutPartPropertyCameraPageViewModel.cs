using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Localization.Video.Common;
using Localization.Video.ViewModels;
using VideoModule.Views;

namespace VideoModule.ViewModels
{
	public class LayoutPartPropertyCameraPageViewModel : SaveCancelDialogViewModel
	{
		public LayoutPartPropertyCameraPageViewModel(UIElement uiElement)
		{
			Title = CommonResources.CameraList;
			PropertyViewModels = new ObservableCollection<PropertyViewModel>();
			var vlcControlViews = new List<VlcControlView>();
			LayoutMultiCameraView.GetLogicalChildCollection(uiElement, vlcControlViews);
			int i = 0;
			foreach (var control in vlcControlViews)
			{
				var presentationCellName = string.Format(CommonViewModels.Window, ++i);
				var item = ClientSettings.RviMultiLayoutCameraSettings.Dictionary.FirstOrDefault(x => x.Key == control.Name);
				if (item.Value == Guid.Empty)
					PropertyViewModels.Add(new PropertyViewModel(control.Name, presentationCellName, Guid.Empty));
				else
					PropertyViewModels.Add(new PropertyViewModel(item.Key, presentationCellName, item.Value));
			}
			OnPropertyChanged(() => PropertyViewModels);
		}

		public ObservableCollection<PropertyViewModel> PropertyViewModels { get; private set; }
	}
}