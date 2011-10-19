using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace InstallerLibrary
{
    public static class CheckSQLServer
    {
        public static bool CheckSQLExpress()
        {
            return (isExpressInstalled());
        }

        static bool isExpressInstalled()
        {
            const string edition = "Express Edition";
            const string instance = "MSSQL$SQLEXPRESS";
            const int spLevel = 1;

            bool fCheckEdition = false;
            bool fCheckSpLevel = false;

            try
            {
                // Run a WQL query to return information about SKUNAME and SPLEVEL about installed instances
                // of the SQL Engine.
                ManagementObjectSearcher getSqlExpress =
                    new ManagementObjectSearcher("root\\Microsoft\\SqlServer\\ComputerManagement",
                    "select * from SqlServiceAdvancedProperty where SQLServiceType = 1 and ServiceName = '"
                    + instance + "' and (PropertyName = 'SKUNAME' or PropertyName = 'SPLEVEL')");

                // If nothing is returned, SQL Express isn't installed.
                if (getSqlExpress.Get().Count == 0)
                {
                    return false;
                }

                // If something is returned, verify it is the correct edition and SP level.
                foreach (ManagementObject sqlEngine in getSqlExpress.Get())
                {
                    if (sqlEngine["ServiceName"].ToString().Equals(instance))
                    {
                        switch (sqlEngine["PropertyName"].ToString())
                        {
                            case "SKUNAME":
                                // Check if this is Express Edition or Express Edition with Advanced Services
                                fCheckEdition = sqlEngine["PropertyStrValue"].ToString().Contains(edition);
                                break;

                            case "SPLEVEL":
                                // Check if the instance matches the specified level
                                fCheckSpLevel = int.Parse(sqlEngine["PropertyNumValue"].ToString()) >= spLevel;
                                //fCheckSpLevel = sqlEngine["PropertyNumValue"].ToString().Contains(spLevel);
                                break;
                        }
                    }
                }

                if (fCheckEdition & fCheckSpLevel)
                {

                    return true;
                }
                return false;
            }
            catch (ManagementException e)
            {
                Console.WriteLine("Error: " + e.ErrorCode + ", " + e.Message);
                return false;
            }
        }
    }
}
