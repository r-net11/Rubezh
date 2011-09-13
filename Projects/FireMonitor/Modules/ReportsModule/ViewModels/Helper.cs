using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;

namespace ReportsModule
{
    public static class Helper
    {
        public static void AddDataColumns<T>(DataTable dataTable)
        {
            var propertyInfos = typeof(T).GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                dataTable.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
            }
        }
        public static Size Subtract(this Size s1, Size s2)
        {
            return new Size(s1.Width - s2.Width, s1.Height - s2.Height);
        }
    }
}
