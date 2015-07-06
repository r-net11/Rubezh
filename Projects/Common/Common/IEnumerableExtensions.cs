using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
	public static class IEnumerableExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			if (source != null)
				foreach (T item in source)
					action(item);
		}

		public static bool IsEmpty<T>(this IEnumerable<T> source)
		{
			return source == null || source.Count() == 0;
		}

		public static bool IsEmpty<T>(this List<T> source)
		{
			return source == null || source.Count == 0;
		}

		public static bool IsEmpty(this string source)
		{
			return string.IsNullOrWhiteSpace(source);
		}
	}
}