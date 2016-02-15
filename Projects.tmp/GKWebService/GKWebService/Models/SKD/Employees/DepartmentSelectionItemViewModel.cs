using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.SKD;

namespace GKWebService.Models.SKD.Employees
{
    public class DepartmentSelectionItemViewModel
    {
        public Guid UID { get; set; }
        public ShortDepartment Model { get; set; }
        public string Name { get; set; }
        public bool IsExpanded { get; set; }
        public int Level { get; set; }
        public Guid ParentUID { get; set; }
        public bool IsLeaf { get; set; }

        public DepartmentSelectionItemViewModel()
        {
            
        }

        public DepartmentSelectionItemViewModel(ShortDepartment department)
        {
            Model = department;
            UID = Model.UID;
            Name = Model.Name;
        }
    }
}