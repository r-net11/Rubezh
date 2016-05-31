using System;

namespace StrazhAPI.Extensions
{
	public static class GuidExtensions
	{
		public static bool IsNullOrEmpty(this Guid? id)
		{
			return id == null || id == Guid.Empty;
		}
	}
}
