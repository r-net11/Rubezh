using System.Collections.Generic;
using System.Text;

namespace RubezhService.Report
{
	internal static class SqlBuilder
	{
		public const string OR = " OR ";
		public const string AND = " AND ";
		public const string WHERE = " WHERE ";
		public const string ORDERBY = " ORDER BY ";
		public const string ASC = " ASC";
		public const string DESC = " DESC";

		public static void BuildConditionOR<T>(StringBuilder sb, string field, IEnumerable<T> objects)
		{
			BuildCondition(sb, field, OR, objects);
		}
		public static void BuildConditionAND<T>(StringBuilder sb, string field, IEnumerable<T> objects)
		{
			BuildCondition(sb, field, AND, objects);
		}
		public static void BuildCondition<T>(StringBuilder sb, string field, string join, IEnumerable<T> objects)
		{
			if (objects == null)
				return;
			var first = true;
			foreach (var obj in objects)
				if (first)
				{
					sb.AppendFormat("({0}='{1}'", field, obj);
					first = false;
				}
				else
					sb.AppendFormat("{0}{1}='{2}'", join, field, obj);
			if (!first)
				sb.Append(")");
		}
	}
}
