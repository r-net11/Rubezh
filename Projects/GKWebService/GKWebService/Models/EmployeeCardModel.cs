using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.SKD;

namespace GKWebService.Models
{
    public class EmployeeCardModel
    {
		public uint Number { get; set; }

		public List<AccessDoorModel> Doors { get; set; }

		public static EmployeeCardModel Create(SKDCard card)
        {
			var model = new EmployeeCardModel();
			model.Number = card.Number;
            return model;
        }
    }
}