using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using StrazhAPI.SKD;

namespace StrazhAPI.Extensions
{
	public static class AdditionalColumnExtensions
	{
		//public static DataColumn ToDataColumn(this AdditionalColumn column)
		//{
		//	return new DataColumn(column.TextData, column.AdditionalColumnType.ToType());
		//}

		public static DataColumn ToDataColumn(this AdditionalColumnType column)
		{
			return new DataColumn(column.Name, column.ToType());
		}

		public static Type ToType(this AdditionalColumnType columnType)
		{
			switch (columnType.DataType)
			{
				case AdditionalColumnDataType.Text:
					return typeof (string);
				case AdditionalColumnDataType.Graphics:
					return typeof (byte[]);
				default:
					return typeof (string);
			}
		}
	}
}
