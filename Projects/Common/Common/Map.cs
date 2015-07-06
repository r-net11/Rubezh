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
			return (_map != null && _map.ContainsKey(guid)) ? _map[guid] : default(T);
		}

		public T GetItem<TT>(TT item, Func<TT, Guid> keySelector)
		{
			var key = keySelector(item);
			return GetItem(key);
		}

		public ICollection<T> Values
		{
			get { return _map.Values; }
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
			var sdfsd = items;
			foreach (var item in items)
			{
				int ss = sdfsd.ToList().FindAll(x => x.UID == item.UID).Count;
				if (ss > 1)
				{
					var s = sdfsd.ToList().FindLastIndex(x => x.UID == new Guid("3ad99076-ed84-41b8-8b0b-3f170c55ce2c"));
				}
			}
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

		#endregion IMap Members
	}

	internal interface IMap
	{
		void Build();

		void BuildSafe();
	}
}