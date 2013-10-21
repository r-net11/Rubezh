using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.ViewModels;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Services.DragDrop;
using FiresecClient;
using Infrastructure;

namespace LayoutModule.ViewModels
{
	public class MonitorLayoutsTreeViewModel : BaseViewModel
	{
		public MonitorLayoutsTreeViewModel(MonitorLayoutsViewModel layouts)
		{
			Layouts = layouts;
			TreeNodeDropCommand = new RelayCommand<TreeNodeDropObject>(OnDrop, CanDrop);
		}

		public MonitorLayoutsViewModel Layouts { get; private set; }

		public RelayCommand<TreeNodeDropObject> TreeNodeDropCommand { get; private set; }
		private void OnDrop(TreeNodeDropObject data)
		{
			var source = data.DataObject.GetData(typeof(LayoutViewModel)) as LayoutViewModel;
			var target = data.Target as LayoutViewModel;
			Layouts.SelectedLayout = source;
			Layouts.LayoutCutCommand.Execute();
			Layouts.SelectedLayout = target;
			Layouts.LayoutPasteCommand.Execute(target == null);
		}
		private bool CanDrop(TreeNodeDropObject data)
		{
			var source = data.DataObject.GetData(typeof(LayoutViewModel)) as LayoutViewModel;
			if (source == null)
				return false;
			var target = data.Target as LayoutViewModel;
			if (target == null && source.Parent != null)
				return true;
			if (target == source || target == source.Parent || target.GetAllParents().Contains(source) || target.IsLayout)
				return false;
			return true;
		}
	}
}