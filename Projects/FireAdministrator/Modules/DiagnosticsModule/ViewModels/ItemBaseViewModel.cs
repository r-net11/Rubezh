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

		public static ItemBaseViewModel<T> Create(T t)
		{
			return new ItemBaseViewModel<T>(t) { Item = t };
		}
	}
}