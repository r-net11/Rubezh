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
			PropertyViewModels = new ObservableCollection<PropertyViewModel>();
			var controls = new List<CellPlayerWrap>();
			GetLogicalChildCollection(uiElement, controls);
			foreach (var control in controls)
			{
				var item = ClientSettings.RviMultiLayoutCameraSettings.Dictionary.FirstOrDefault(x => x.Key == control.Name);
				if (item.Value == Guid.Empty)
					PropertyViewModels.Add(new PropertyViewModel(control.Name, control.Camera == null ? Guid.Empty : control.Camera.UID));
				else
					PropertyViewModels.Add(new PropertyViewModel(item.Key, item.Value));
			}
		}

		private ObservableCollection<PropertyViewModel> _propertyViewModels;
		public ObservableCollection<PropertyViewModel> PropertyViewModels
		{
			get { return _propertyViewModels; }
			set
			{
				_propertyViewModels = value;
				OnPropertyChanged(() => PropertyViewModels);
			}
		}

		private static void GetLogicalChildCollection(DependencyObject parent, List<CellPlayerWrap> logicalCollection)
		{
			var children = LogicalTreeHelper.GetChildren(parent);
			foreach (var child in children)
			{
				if (child is DependencyObject)
				{
					var depChild = child as DependencyObject;
					if (child is CellPlayerWrap)
					{
						logicalCollection.Add(child as CellPlayerWrap);
					}
					GetLogicalChildCollection(depChild, logicalCollection);
				}
			}
		}

		protected override bool CanSave()
		{
			return true;
		}

		protected override bool Save()
		{
			return true;
		}
	}
}