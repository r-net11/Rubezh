using System;
using System.Text;

namespace Common
{
	public static class GuidHelper
	{
		public static string ToString(Guid guid)
		{
			if (guid == Guid.Empty)
				return null;
			return guid.ToString();
		}

		public static Guid ToGuid(string val)
		{
			if (val == null)
				return Guid.Empty;
			return new Guid(val);
		}

		public static Guid CreateOn(Guid guid, int index)
		{
			var stringUID = guid.ToString();
			var firstChar = stringUID[index];
			if (firstChar == '0')
				firstChar = '1';
			else
				firstChar = '0';

			var stringBuilder = new StringBuilder(stringUID);
			stringBuilder[index] = firstChar;
			var newStringUID = stringBuilder.ToString();

			var newGuid = new Guid(newStringUID);
			return newGuid;
		}
	}
}