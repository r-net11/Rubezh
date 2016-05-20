using Infrastructure.Common.TreeList;
using RubezhAPI.GK;
using RubezhAPI.Hierarchy;

namespace DiagnosticsModule.ViewModels
{
	public class ItemBaseViewModel<T> : TreeNodeViewModel<ItemBaseViewModel<T>> where T : ModelBase
	{
		public HierarchicalItem<T> HierarchicalItem { get; private set; }
		public T Item { get; private set; }

		public ItemBaseViewModel()
		{
		}

		public ItemBaseViewModel(HierarchicalItem<T> t)
		{
			HierarchicalItem = t;
			Item = t.Item;
		}
	}
}