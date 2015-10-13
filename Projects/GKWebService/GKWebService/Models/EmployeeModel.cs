using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.SKD;

namespace GKWebService.Models
{
    public class EmployeeModel
    {
        public Guid UID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public Photo Photo { get; set; }
        public List<AdditionalColumn> AdditionalColums { get; set; }


        public static EmployeeModel CreateFromEmployee(Employee employee)
        {
            return new EmployeeModel
            {
                FirstName = employee.FirstName,
                SecondName = employee.SecondName,
                LastName = employee.LastName,
                Description = employee.Description,
                UID = employee.UID,
                Photo = employee.Photo,
                AdditionalColums = employee.AdditionalColumns,
            };
        }
    }
}