using System;
using System.Collections.Generic;

namespace Common
{
	public class MapSource
	{
		private Dictionary<Type, object> _sourceMap;

		public MapSource()
		{
			_sourceMap = new Dictionary<Type, object>();
		}

		public void Add<T>(Func<IEnumerable<T>> builder = null)
			where T : IIdentity
		{
			if (_sourceMap.ContainsKey(typeof(T)))
				_sourceMap.Add(typeof(T), new Map<T>(builder));
			else
				_sourceMap[typeof(T)] = new Map<T>(builder);
		}
		public void BuildSafe<T>(IEnumerable<T> items = null)
			where T : IIdentity
		{
			if (_sourceMap.ContainsKey(typeof(T)))
				((Map<T>)_sourceMap[typeof(T)]).BuildSafe(items);
		}

		public void BuildAll()
		{
			_sourceMap.ForEach(pair => ((IMap)pair.Value).Build());
		}
		public void BuildAllSafe()
		{
			_sourceMap.ForEach(pair => ((IMap)pair.Value).BuildSafe());
		}

		public T Get<T>(Guid uid)
			where T : IIdentity
		{
			return _sourceMap.ContainsKey(typeof(T)) ? ((Map<T>)_sourceMap[typeof(T)]).GetItem(uid) : default(T);
		}
	}
}