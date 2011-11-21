using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using System.Diagnostics;

namespace CustomActions
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CreateSQLInstanceList(Session session)
        {
            session.Log("Begin CustomAction");
            Debugger.Break();
            //InstanceListView instanceListView = new InstanceListView();
            //var result = instanceListView.ShowDialog();
            //session["SQLInstanceList"] = "Error";
            //if (result == true)
            //{
            //    session["SQLInstanceList"] = instanceListView.SelectedInstance;
            //}
            try
            {
                //String val = (String)session["SOME_KEY"];
                int rowCnt = session.Database.CountRows("ListBox"); //TODO add where's clause
                View myView = session.Database.OpenView("SELECT * FROM `ListBox` WHERE (`Property`='SQLINSTANCE_LIST')");
                Record record = new Record("SQLINSTANCE_LIST", rowCnt);
                record[0] = "SQLINSTANCE_LIST";
                record[1] = 4;
                record[2] = "storeName";
                //record[3] = "storeName";
                myView.Insert(record);
                myView.Close();
                myView.Dispose();
            }
            catch (Exception ex)
            {
                return (ActionResult.Failure);
            }
            return (ActionResult.Success);
        }

        public List<object> GenSQLInstanceList()
        {
            List<object> objects = new List<object>();
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names");
            foreach (string sk in key.GetSubKeyNames())
            {
                RegistryKey rkey = key.OpenSubKey(sk);

                foreach (string s in rkey.GetValueNames())
                {
                    objects.Add(s);
                }
            }
            return objects;
        }

        private static void InsertRecord(Session session, string tableName, Object[] objects)
        {

            Database db = session.Database;

            string sqlInsertSring = db.Tables[tableName].SqlInsertString + " TEMPORARY";

            View view = db.OpenView(sqlInsertSring);

            view.Execute(new Record(objects));

            view.Close();



        }

        
    }
}
