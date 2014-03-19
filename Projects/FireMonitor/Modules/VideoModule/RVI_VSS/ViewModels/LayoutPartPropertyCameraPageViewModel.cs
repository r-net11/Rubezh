using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Infrastructure;
using Infrastructure.Common.Video.RVI_VSS;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.RVI_VSS.ViewModels
{
	public class LayoutPartPropertyCameraPageViewModel : SaveCancelDialogViewModel
	{
		public ObservableCollection<PropertyViewModel> PropertyViewModels { get; private set; }

		public LayoutPartPropertyCameraPageViewModel(UIElement uiElement)
		{
			Title = "Список камер";
			PropertyViewModels = new ObservableCollection<PropertyViewModel>();
			var cellPlayerWraps = new List<CellPlayerWrap>();
			Views.LayoutMultiCameraView.GetLogicalChildCollection(uiElement, cellPlayerWraps);
			foreach (var control in cellPlayerWraps)
			{
				var item = ClientSettings.RviMultiLayoutCameraSettings.Dictionary.FirstOrDefault(x => x.Key == control.Name);
				if (item.Value == Guid.Empty)
					PropertyViewModels.Add(new PropertyViewModel(control.Name, Guid.Empty));
				else
					PropertyViewModels.Add(new PropertyViewModel(item.Key, item.Value));
			}
			OnPropertyChanged("PropertyViewModels");
		}
	}
}