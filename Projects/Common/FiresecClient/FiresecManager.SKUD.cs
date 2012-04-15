using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models.Skud;

namespace FiresecClient
{
    public partial class FiresecManager
    {
		public static IEnumerable<EmployeeCard> GetEmployees()
        {
            return FiresecService.GetEmployees();
        }

    }
}