using Infrastructure.Common.TreeList;
using RubezhAPI.GK;

namespace DiagnosticsModule.ViewModels
{
	public class ItemBaseViewModel<T> : TreeNodeViewModel<ItemBaseViewModel<T>> where T : ModelBase
	{
		public T Item { get; private set; }

		public ItemBaseViewModel()
		{
		}

		public ItemBaseViewModel(T t)
		{
			Item = t;
		}
	}
}