using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Common
{
	public class MapSource
	{
		private Dictionary<Type, object> _sourceMap;
		private MethodInfo _addIdentityMapMethod;
		private MethodInfo _addMapMethod;

		public MapSource()
		{
			_sourceMap = new Dictionary<Type, object>();
			_addMapMethod = GetType().GetMethod("AddMap");
			_addIdentityMapMethod = GetType().GetMethod("AddIdentityMap", new Type[] { });
		}
		public MapSource(params Type[] types)
		{
			types.ForEach(type =>
			{
				if (typeof(IIdentity).IsAssignableFrom(type))
				{
					var generic = _addIdentityMapMethod.MakeGenericMethod(type);
					generic.Invoke(this, null);
				}
				else
				{
					var generic = _addMapMethod.MakeGenericMethod(type);
					generic.Invoke(this, null);
				}
			});
		}

		public void Add<T, TT>()
		{
			Add<T>();
			Add<TT>();
		}
		public void Add<T, TT, TTT>()
		{
			Add<T>();
			Add<TT>();
			Add<TTT>();
		}
		public void Add<T, TT, TTT, TTTT>()
		{
			Add<T>();
			Add<TT>();
			Add<TTT>();
			Add<TTTT>();
		}
		public void Add<T>()
		{
			if (typeof(IIdentity).IsAssignableFrom(typeof(T)))
			{
				var generic = _addIdentityMapMethod.MakeGenericMethod(typeof(T));
				generic.Invoke(this, null);
			}
			else
				AddMap<T>();
		}

		public void AddMap<T>()
		{
			_sourceMap.Add(typeof(T), new Map<T>());
		}
		public void AddIdentityMap<T>()
			where T : IIdentity
		{
			_sourceMap.Add(typeof(T), new IdentityMap<T>());
		}
		public void AddIdentityMap<T>(Func<IEnumerable<T>> builder)
			where T : IIdentity
		{
			_sourceMap.Add(typeof(T), new IdentityMap<T>(builder));
		}
		public void Build<T>(IEnumerable<T> items, Func<T, Guid> keySelector)
		{
			if (_sourceMap.ContainsKey(typeof(T)))
				((Map<T>)_sourceMap[typeof(T)]).Build(items, keySelector);
		}
		public void Build<T>(IEnumerable<T> items = null)
			where T : IIdentity
		{
			if (_sourceMap.ContainsKey(typeof(T)))
				((IdentityMap<T>)_sourceMap[typeof(T)]).Build(items);
		}

		public void BuildAll()
		{
			_sourceMap.ForEach(pair =>
			{
				if (typeof(IIdentity).IsAssignableFrom(pair.Key))
				{
					var method = pair.Value.GetType().GetMethod("BuildIdentity", BindingFlags.Instance | BindingFlags.NonPublic);
					method.Invoke(pair.Value, new object[] { });
				}
			});
		}
		public void BuildAllSafe()
		{
			_sourceMap.ForEach(pair =>
			{
				if (typeof(IIdentity).IsAssignableFrom(pair.Key))
				{
					var method = pair.Value.GetType().GetMethod("BuildIdentitySafe", BindingFlags.Instance | BindingFlags.NonPublic);
					method.Invoke(pair.Value, new object[] { });
				}
			});
		}

		public T Get<T>(Guid uid)
		{
			return _sourceMap.ContainsKey(typeof(T)) ? ((Map<T>)_sourceMap[typeof(T)]).GetItem(uid) : default(T);
		}
	}
}
