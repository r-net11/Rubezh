using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using SettingsModule;

namespace SettingsModule
{
    public class Db_and_Query
    {
        //#region Construction
        //public Db_and_Query()
        //{
        //    sampleDb = new SampleDatabaseEntities();
        //}
        //#endregion

        //#region Fields
        //SampleDatabaseEntities sampleDb;
        //#endregion
        
        //#region Methods
        //public void AddRows(int n)
        //{
        //    for (int i = 0; i < n; i++)
        //    {
        //        AddRow();
        //    }
        //}
        //public void AddRow()
        //{
        //    Gk_event row = new Gk_event();
        //    row.key = this.sampleDb.Gk_events.Count();

        //    row.device = GetRandomDevice();
        //    row.date = GetRandomDate();

        //    this.sampleDb.Gk_events.AddObject(row);
        //    this.sampleDb.SaveChanges();
        //}
        //private static DateTime GetRandomDate()
        //{
        //    Random rnd = new Random();
        //    DateTime dt;
        //    string str = rnd.Next(31).ToString() + "." + rnd.Next(12).ToString() + "." + (2000 + rnd.Next(12)).ToString();
        //    if (DateTime.TryParse(str, out dt))
        //        return dt;
        //    else
        //        return DateTime.Parse("01.01.1900");
        //}
        //private static string GetRandomDevice()
        //{
        //    Random rnd = new Random();
        //    int tmp = rnd.Next(5);
        //    switch (tmp)
        //    {
        //        case 0:
        //            return "IP";
        //        case 1:
        //            return "RM";
        //        case 2:
        //            return "MDU";
        //        case 4:
        //            return "MPT";
        //        default:
        //            return "undefined";
        //    }
        //}
        //public List<Gk_event> Select_devices()
        //{
        //    var query =
        //        from t in sampleDb.Gk_events
        //        orderby t.key
        //        select t;
        //    return query.ToList();
        //}
        //public List<Gk_event> Select_devices(List<string> filter)
        //{
        //    var query =
        //        from t in sampleDb.Gk_events
        //        where filter.Contains(t.device)
        //        orderby t.key
        //        select t;
        //    return query.ToList();
        //}
        //public List<Gk_event> Select_devices(DateTime starting_date, DateTime ending_date)
        //{
        //    var query =
        //        from t in sampleDb.Gk_events
        //        where t.date >= starting_date && t.date <= ending_date
        //        orderby t.key
        //        select t;
        //    return query.ToList();
        //}
        //public List<Gk_event> Select_devices(List<string> filter, DateTime starting_date, DateTime ending_date)
        //{
        //    var query =
        //        from t in sampleDb.Gk_events
        //        where filter.Contains(t.device)
        //        where t.date >= starting_date && t.date <= ending_date
        //        orderby t.key
        //        select t;
        //    return query.ToList();
        //}
        //#endregion

        ////#region Classes
        ////public class Representer
        ////{
        ////    public int key;
        ////    public string device;
        ////    public DateTime date;
        ////}
        ////#endregion

    }

    
            
}
