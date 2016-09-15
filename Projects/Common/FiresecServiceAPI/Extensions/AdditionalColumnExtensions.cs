using System;
using System.Data;
using StrazhAPI.SKD;

namespace StrazhAPI.Extensions
{
	public static class AdditionalColumnExtensions
	{
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
