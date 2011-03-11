using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;

namespace DiagramDesigner
{
    public class SampleData
    {
        public static DataSource GetSampleData()
        {
            DataItem dataItem1 = new DataItem();
            dataItem1.Name = "Item1";
            dataItem1.DataValue = "Hello";
            dataItem1.Id = Data.IdManager.Next;
            DataItem dataItem2 = new DataItem();
            dataItem2.Name = "Item2";
            dataItem2.DataValue = "Ho, there";
            dataItem2.Id = Data.IdManager.Next;
            DataItem dataItem3 = new DataItem();
            dataItem3.Name = "Item3";
            dataItem3.DataValue = "Arnold";
            dataItem3.Id = Data.IdManager.Next;
            DataSource dataSource = new DataSource();
            dataSource.DataItems = new List<DataItem>();
            dataSource.DataItems.Add(dataItem1);
            dataSource.DataItems.Add(dataItem2);
            dataSource.DataItems.Add(dataItem3);

            return dataSource;
        }
    }
}
