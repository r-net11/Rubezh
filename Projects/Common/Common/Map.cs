using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
	public class Map<T>
	{
		private Dictionary<Guid, T> _map;

		public T GetItem(Guid guid)
		{
			return _map.ContainsKey(guid) ? _map[guid] : default(T);
		}
		public T GetItem<TT>(TT item, Func<TT, Guid> keySelector)
		{
			var key = keySelector(item);
			return GetItem(key);
		}

		public void BuildSafe(IEnumerable<T> items, Func<T, Guid> keySelector)
		{
			_map = items == null ? new Dictionary<Guid, T>() : items.GroupBy(keySelector).ToDictionary(group => group.Key, group => group.First());
		}
		public void Build(IEnumerable<T> items, Func<T, Guid> keySelector)
		{
			_map = items == null ? new Dictionary<Guid, T>() : items.ToDictionary(keySelector);
		}
	}
	public class IdentityMap<T> : Map<T>
		where T : IIdentity
	{
		public Func<IEnumerable<T>> Builder { get; private set; }

		public IdentityMap(Func<IEnumerable<T>> builder = null)
		{
			Builder = builder;
		}

		public void BuildSafe(IEnumerable<T> items = null)
		{
			if (items == null && Builder != null)
				items = Builder();
			BuildSafe(items, item => item.UID);
		}
		public void Build(IEnumerable<T> items = null)
		{
			if (items == null && Builder != null)
				items = Builder();
			Build(items, item => item.UID);
		}

		private void BuildIdentitySafe()
		{
			BuildSafe();
		}
		private void BuildIdentity()
		{
			Build();
		}
	}
}
