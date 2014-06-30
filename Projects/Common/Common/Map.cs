using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
	public class Map<T> : IMap
		where T : IIdentity
	{
		private Dictionary<Guid, T> _map;

		public Func<IEnumerable<T>> Builder { get; private set; }

		public Map(Func<IEnumerable<T>> builder = null)
		{
			Builder = builder;
		}

		public T GetItem(Guid guid)
		{
			return _map.ContainsKey(guid) ? _map[guid] : default(T);
		}
		public T GetItem<TT>(TT item, Func<TT, Guid> keySelector)
		{
			var key = keySelector(item);
			return GetItem(key);
		}

		public void BuildSafe(IEnumerable<T> items = null)
		{
			if (items == null && Builder != null)
				items = Builder();
			_map = items == null ? new Dictionary<Guid, T>() : items.GroupBy(KeySelecter).ToDictionary(group => group.Key, group => group.First());
		}
		public void Build(IEnumerable<T> items = null)
		{
			if (items == null && Builder != null)
				items = Builder();
			_map = items == null ? new Dictionary<Guid, T>() : items.ToDictionary(KeySelecter);
		}

		private Guid KeySelecter(T item)
		{
			return item.UID;
		}

		#region IMap Members

		void IMap.Build()
		{
			Build();
		}

		void IMap.BuildSafe()
		{
			BuildSafe();
		}

		#endregion
	}
	internal interface IMap
	{
		void Build();
		void BuildSafe();
	}
}
