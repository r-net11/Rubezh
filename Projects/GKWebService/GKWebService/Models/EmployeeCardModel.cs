using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.GK;
using RubezhAPI.SKD;

namespace GKWebService.Models
{
    public class EmployeeCardModel
    {
		public SKDCard Card { get; set; }

		public List<GKSchedule> Schedules { get; set; }
 
		public List<SKDCard> StopListCards { get; set; }

		public List<GKControllerModel> AvailableGKControllers { get; set; }

		public List<AccessDoorModel> Doors { get; set; }

		public AccessDoorModel SelectedDoor { get; set; }
	}
}