using System;
using System.Collections.Generic;

namespace Common
{
	public class MapSource
	{
		private readonly Dictionary<Type, object> _sourceMap;

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

		public void Add<T, TT>(Func<IEnumerable<T>> builderT = null, Func<IEnumerable<TT>> builderTT = null)
			where T : IIdentity
			where TT : IIdentity
		{
			Add<T>(builderT);
			Add<TT>(builderTT);
		}

		public void Add<T, TT, TTT>(Func<IEnumerable<T>> builderT = null, Func<IEnumerable<TT>> builderTT = null, Func<IEnumerable<TTT>> builderTTT = null)
			where T : IIdentity
			where TT : IIdentity
			where TTT : IIdentity
		{
			Add<T>(builderT);
			Add<TT>(builderTT);
			Add<TTT>(builderTTT);
		}

		public void Add<T, TT, TTT, TTTT>(Func<IEnumerable<T>> builderT = null, Func<IEnumerable<TT>> builderTT = null, Func<IEnumerable<TTT>> builderTTT = null, Func<IEnumerable<TTTT>> builderTTTT = null)
			where T : IIdentity
			where TT : IIdentity
			where TTT : IIdentity
			where TTTT : IIdentity
		{
			Add<T>(builderT);
			Add<TT>(builderTT);
			Add<TTT>(builderTTT);
			Add<TTTT>(builderTTTT);
		}

		public bool Contains<T>()
		{
			return _sourceMap.ContainsKey(typeof(T));
		}

		public void Build<T>(IEnumerable<T> items = null)
			where T : IIdentity
		{
			if (_sourceMap.ContainsKey(typeof(T)))
				((Map<T>)_sourceMap[typeof(T)]).Build(items);
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

		public Map<T> Map<T>()
				where T : IIdentity
		{
			return _sourceMap.ContainsKey(typeof(T)) ? (Map<T>)_sourceMap[typeof(T)] : null;
		}

		public ICollection<T> Items<T>()
				where T : IIdentity
		{
			return _sourceMap.ContainsKey(typeof(T)) ? ((Map<T>)_sourceMap[typeof(T)]).Values : null;
		}
	}
}