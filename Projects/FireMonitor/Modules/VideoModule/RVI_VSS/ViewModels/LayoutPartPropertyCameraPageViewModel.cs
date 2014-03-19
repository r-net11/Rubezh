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
		public LayoutPartPropertyCameraPageViewModel(UIElement uiElement)
		{
			Title = "Список камер";
			PropertyViewModels = new ObservableCollection<PropertyViewModel>();
			var cellPlayerWraps = new List<CellPlayerWrap>();
			Views.LayoutMultiCameraView.GetLogicalChildCollection(uiElement, cellPlayerWraps);
			int i = 0;
			foreach (var control in cellPlayerWraps)
			{
				var presentationCellName = "Окно " + ++i;
				var item = ClientSettings.RviMultiLayoutCameraSettings.Dictionary.FirstOrDefault(x => x.Key == control.Name);
				if (item.Value == Guid.Empty)
					PropertyViewModels.Add(new PropertyViewModel(control.Name, presentationCellName, Guid.Empty));
				else
					PropertyViewModels.Add(new PropertyViewModel(item.Key, presentationCellName, item.Value));
			}
			OnPropertyChanged("PropertyViewModels");
		}

		public ObservableCollection<PropertyViewModel> PropertyViewModels { get; private set; }
	}
}