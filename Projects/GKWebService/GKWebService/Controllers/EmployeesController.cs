using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using GKWebService.Models;
using GKWebService.Utils;

namespace GKWebService.Controllers
{
    public class EmployeesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EmployeeDetails()
        {
            return View();
        }

		public JsonNetResult GetEmployeeDetails(Guid? id)
		{
			Employee employee;
			if (id.HasValue)
			{
				var operationResult = ClientManager.FiresecService.GetEmployeeDetails(id.Value);
				employee = operationResult.Result;
			}
			else
			{
				employee = new Employee();
				employee.BirthDate = DateTime.Now;
				employee.CredentialsStartDate = DateTime.Now;
				employee.DocumentGivenDate = DateTime.Now;
				employee.DocumentValidTo = DateTime.Now;
				employee.RemovalDate = DateTime.Now;
				employee.ScheduleStartDate = DateTime.Now;
			}
			employee.Photo = null;
			employee.AdditionalColumns.ForEach(c => c.Photo = null);
			return new JsonNetResult { Data = employee };
		}

		[HttpPost]
		public JsonNetResult EmployeeDetails(Employee employee, bool isNew)
        {
			var operationResult = ClientManager.FiresecService.SaveEmployee(employee, isNew);

			return new JsonNetResult { Data = operationResult.Result };
        }

        [HttpPost]
		public JsonNetResult SaveChief(SaveChiefParams @params)
        {
			var result = ClientManager.FiresecService.SaveOrganisationChief(@params.OrganisationUID, @params.EmployeeUID, @params.OrganisationName);

			return new JsonNetResult { Data = !result.HasError };
        }

        [HttpPost]
		public JsonNetResult SaveHRChief(SaveChiefParams @params)
        {
			var result = ClientManager.FiresecService.SaveOrganisationHRChief(@params.OrganisationUID, @params.EmployeeUID, @params.OrganisationName);

			return new JsonNetResult { Data = !result.HasError };
		}

		[HttpPost]
		public JsonResult GetOrganisations(EmployeeFilter employeeFilter)
        {
            var employeeModels = new List<ShortEmployeeModel>();
    
			var organisationFilter = new OrganisationFilter ();
			var organisations = ClientManager.FiresecService.GetOrganisations(organisationFilter).Result;
			var initializedOrganisations = InitializeOrganisations(organisations);

			var employees = ClientManager.FiresecService.GetEmployeeList(employeeFilter).Result;
			var initializedEmployees = InitializeEmployees(employees, initializedOrganisations);
			
	        foreach (var organisation in initializedOrganisations)
	        {
				employeeModels.Add(organisation);
		        employeeModels.AddRange(initializedEmployees.Where(e => e.OrganisationUID == organisation.UID));
			}

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = employeeModels,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
		public JsonNetResult MarkDeleted(Guid uid, string name, bool isOrganisation)
		{
			var operationResult = ClientManager.FiresecService.MarkDeletedEmployee(uid, name, !isOrganisation);
			return new JsonNetResult { Data = operationResult != null && operationResult.HasError && !operationResult.Error.Contains("Ошибка БД") };
		}

		[HttpPost]
		public JsonNetResult Restore(Guid uid, string name, bool isOrganisation)
		{
			var result = ClientManager.FiresecService.RestoreEmployee(uid, name, !isOrganisation);
			return new JsonNetResult { Data = !result.HasError };
		}

        public JsonNetResult GetOrganisation(Guid? id)
        {
			var filter = new OrganisationFilter();
			filter.UIDs.Add(id.Value);
			var operationResult = ClientManager.FiresecService.GetOrganisations(filter);
			return new JsonNetResult { Data = operationResult.Result.FirstOrDefault() };
        }

        private List<ShortEmployeeModel> InitializeEmployees(IEnumerable<ShortEmployee> employees, IEnumerable<ShortEmployeeModel> organisations)
        {
            return employees.Select(e => ShortEmployeeModel.CreateFromModel(e, organisations)).ToList();
        }

        private List<ShortEmployeeModel> InitializeOrganisations(IEnumerable<Organisation> organisations)
        {
            return organisations.Select(ShortEmployeeModel.CreateFromOrganisation).ToList();
        }

		public JsonNetResult GetEmployeeCards(Guid? id)
		{
			var cards = new List<EmployeeCardModel>();
			if (id.HasValue)
			{
				var operationResult = ClientManager.FiresecService.GetEmployeeCards(id.Value);
				cards.AddRange(operationResult.Result.Select(CreateCard));
			}
			return new JsonNetResult { Data = new {Cards = cards}};
		}

		public ActionResult EmployeeCardDetails()
		{
			return View();
		}

		public JsonNetResult GetEmployeeCardDetails(Guid? id)
		{
			SKDCard card;
			if (id.HasValue)
			{
				var operationResult = ClientManager.FiresecService.GetSingleCard(id.Value);
				card = operationResult.Result;
			}
			else
			{
				card = new SKDCard();
			}

			return new JsonNetResult { Data = card };
		}

		[HttpPost]
		public JsonNetResult EmployeeCardDetails(SKDCard card, string employeeName, bool isNew)
		{
			var operationResult = ClientManager.FiresecService.EditCard(card, employeeName);

			return new JsonNetResult { Data = operationResult.Result };
		}

		private EmployeeCardModel CreateCard(SKDCard card)
		{
			var employeeCard = EmployeeCardModel.Create(card);
			var cardDoors = GetCardDoors(card);
			employeeCard.Doors = InitializeDoors(cardDoors);
			return employeeCard;
		}

	    private List<CardDoor> GetCardDoors(SKDCard card)
		{
			var cardDoors = new List<CardDoor>();
			cardDoors.AddRange(card.CardDoors);
			if (card.AccessTemplateUID != null)
			{
				var result = ClientManager.FiresecService.GetAccessTemplates(new AccessTemplateFilter());
				var accessTemplates = result.Result;
				if (accessTemplates != null)
				{
					var accessTemplate = accessTemplates.FirstOrDefault(x => x.UID == card.AccessTemplateUID);
					if (accessTemplate != null)
					{
						foreach (var cardZone in accessTemplate.CardDoors)
						{
							if (!cardDoors.Any(x => x.DoorUID == cardZone.DoorUID))
								cardDoors.Add(cardZone);
						}
					}
				}
			}
			return cardDoors;
		}

		List<AccessDoorModel> InitializeDoors(IEnumerable<CardDoor> cardDoors)
		{
			var operationResult = ClientManager.FiresecService.GetGKSchedules();
			if (operationResult.Result != null)
				operationResult.Result.ForEach(x => x.ScheduleParts = x.ScheduleParts.OrderBy(y => y.DayNo).ToList());
			var schedules = operationResult.Result;
			var doors = new List<AccessDoorModel>();
			var gkDoors = from cardDoor in cardDoors
						  join gkDoor in GKManager.DeviceConfiguration.Doors on cardDoor.DoorUID equals gkDoor.UID
						  select new { CardDoor = cardDoor, GKDoor = gkDoor };
			foreach (var doorViewModel in gkDoors.Select(x => new AccessDoorModel(x.GKDoor, x.CardDoor, schedules)).OrderBy(x => x.PresentationName))
				doors.Add(doorViewModel);
			return doors;
		}
	}

	public class SaveChiefParams
	{
		public Guid OrganisationUID { get; set; }
		public Guid? EmployeeUID { get; set; }
		public string OrganisationName { get; set; }
	}
}