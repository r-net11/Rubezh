using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ReportsModule.ViewModels
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
    }
}
