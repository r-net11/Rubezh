using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Documents;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using GKWebService.Models;
using GKWebService.Models.SKD.Employees;
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
            var employeeModel = new EmployeeDetailsViewModel()
            {
                Employee = new Employee(),
            };

            employeeModel.Initialize(id);


            return new JsonNetResult { Data = employeeModel };
		}

		[HttpPost]
		public JsonNetResult EmployeeDetails(EmployeeDetailsViewModel employee, bool isNew)
		{
		    var error = employee.Save(isNew);

			return new JsonNetResult { Data = error };
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
    
			var organisationFilter = new OrganisationFilter { UIDs = employeeFilter.OrganisationUIDs, UserUID = ClientManager.CurrentUser.UID, LogicalDeletationType = employeeFilter.LogicalDeletationType };
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

        public JsonNetResult GetEmployeePhoto(Guid id)
        {
            var employeeModel = new EmployeeDetailsViewModel();
            employeeModel.Initialize(id);
            return new JsonNetResult { Data = employeeModel.PhotoData ?? string.Empty};
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
			var cards = new List<ShortEmployeeCardModel>();
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

		public JsonNetResult GetEmployeeCardDetails(Guid? organisationId, Guid? cardId)
		{
			if (!organisationId.HasValue)
			{
				return new JsonNetResult { Data = new EmployeeCardModel
					{
						Card = new SKDCard(),
						//SelectedStopListCard = new SKDCard(),
						//SelectedSchedule = new GKSchedule(),
						//SelectedAccessTemplate = new AccessTemplate(),
					} 
				};
			}

			SKDCard card;
			if (cardId.HasValue)
			{
				card = ClientManager.FiresecService.GetSingleCard(cardId.Value).Result;
			}
			else
			{
				card = new SKDCard
				{
					EndDate = DateTime.Now.AddYears(1),
					GKCardType = GKCardType.Employee,
				};
			}

			var cardModel = new EmployeeCardModel();

			cardModel.Card = card;

			cardModel.Schedules = ClientManager.FiresecService.GetGKSchedules().Result;
			cardModel.SelectedScheduleNo = cardModel.Card.GKLevelSchedule;

			var operationResult = ClientManager.FiresecService.GetCards(new CardFilter { DeactivationType = LogicalDeletationType.Deleted });
			cardModel.StopListCards = operationResult.Result.Where(x => x.IsInStopList).ToList();

			cardModel.AvailableGKControllers = GKManager.Devices.Where(x => x.DriverType == GKDriverType.GK)
																.Select(d =>
																{
																	var isChecked = !cardId.HasValue;
																	isChecked |= card.GKControllerUIDs != null && card.GKControllerUIDs.Contains(d.UID);
																	return new GKControllerModel(d.UID, isChecked, d.PresentationName);
																}).ToList();

			var organisation = ClientManager.FiresecService.GetOrganisations(new OrganisationFilter {UIDs = new List<Guid>{organisationId.Value}}).Result.FirstOrDefault();

			cardModel.Doors = GKManager.DeviceConfiguration.Doors.Where(door => organisation.DoorUIDs.Any(y => y == door.UID))
				.Select(door => new AccessDoorModel(door, card.CardDoors, cardModel.Schedules))
				.ToList();

			var accessTemplateFilter = new AccessTemplateFilter { OrganisationUIDs = new List<Guid> { organisationId.Value } };
			cardModel.AvailableAccessTemplates = new List<AccessTemplate> { new AccessTemplate { UID = Guid.Empty, Name = "<нет>" } }
				.Concat(ClientManager.FiresecService.GetAccessTemplates(accessTemplateFilter).Result)
				.ToList();
			cardModel.SelectedAccessTemplate = cardModel.AvailableAccessTemplates.FirstOrDefault(x => x.UID == cardModel.Card.AccessTemplateUID);

			return new JsonNetResult { Data = cardModel };
		}

		[HttpPost]
		public JsonNetResult EmployeeCardDetails(EmployeeCardModel cardModel, string employeeName, bool isNew)
		{
			cardModel.Save();

			OperationResult<bool> operationResult;

			if (isNew)
			{
				operationResult = ClientManager.FiresecService.AddCard(cardModel.Card, employeeName);
			}
			else
			{
				operationResult = ClientManager.FiresecService.EditCard(cardModel.Card, employeeName);
			}

			return new JsonNetResult { Data = operationResult.Result };
		}

        public ActionResult CardRemovalReason()
        {
            return View();
        }

        [HttpPost]
        public JsonNetResult DeleteCard(Guid id)
        {
            var operationResult = ClientManager.FiresecService.DeletedCard(new SKDCard {UID = id});
            return new JsonNetResult { Data = !operationResult.HasError };
        }

        [HttpPost]
        public JsonNetResult DeleteFromEmployee(Guid id, string employeeName, string reason)
        {
            var operationResult = ClientManager.FiresecService.DeleteCardFromEmployee(new SKDCard { UID = id }, employeeName, reason);
            return new JsonNetResult { Data = !operationResult.HasError };
        }

        public ActionResult DepartmentSelection()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDepartments(Guid organisationUID, Guid? departmentUID)
        {
            var operationResult = ClientManager.FiresecService.GetDepartmentList(new DepartmentFilter
            {
                OrganisationUIDs = new List<Guid> { organisationUID },
                ExceptUIDs = (departmentUID.HasValue && departmentUID.Value != Guid.Empty ? new List<Guid> { departmentUID.Value } : new List<Guid> ())
            });

            if (operationResult.HasError)
            {
                throw new InvalidOperationException(operationResult.Error);
            }

            var departments = new List<DepartmentSelectionItemViewModel>();
            foreach (var rootItem in operationResult.Result.Where(d => d.ParentDepartmentUID == null || d.ParentDepartmentUID == Guid.Empty))
            {
                var itemViewModel = new DepartmentSelectionItemViewModel(rootItem);
                itemViewModel.Level = 0;
                itemViewModel.ParentUID = Guid.Empty;
                itemViewModel.IsLeaf = true;
                departments.Add(itemViewModel);
                int index = departments.IndexOf(itemViewModel);
                AddChildren(departments, itemViewModel, operationResult.Result, ref index);
                itemViewModel.IsExpanded = !itemViewModel.IsLeaf;   // если был добавлен дочерний элемент, то разворачиваем
            }

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = departments,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void AddChildren(List<DepartmentSelectionItemViewModel> result, DepartmentSelectionItemViewModel parentViewModel, IEnumerable<ShortDepartment> models, ref int index)
        {
            if (parentViewModel.Model.ChildDepartments != null && parentViewModel.Model.ChildDepartments.Count > 0)
            {
                var children = models.Where(x => x.ParentDepartmentUID == parentViewModel.Model.UID).ToList();
                foreach (var child in children)
                {
                    var itemViewModel = new DepartmentSelectionItemViewModel(child);
                    itemViewModel.Level = parentViewModel.Level + 1;
                    itemViewModel.ParentUID = parentViewModel.UID;
                    itemViewModel.IsLeaf = true;
                    parentViewModel.IsLeaf = false;
                    result.Insert(index + 1, itemViewModel);
                    index++;
                    AddChildren(result, itemViewModel, models, ref index);
                    itemViewModel.IsExpanded = !itemViewModel.IsLeaf;   // если был добавлен дочерний элемент, то разворачиваем
                }
            }
        }

        private ShortEmployeeCardModel CreateCard(SKDCard card)
		{
			var employeeCard = ShortEmployeeCardModel.Create(card);
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

		List<ReadOnlyAccessDoorModel> InitializeDoors(IEnumerable<CardDoor> cardDoors)
		{
			var operationResult = ClientManager.FiresecService.GetGKSchedules();
			if (operationResult.Result != null)
				operationResult.Result.ForEach(x => x.ScheduleParts = x.ScheduleParts.OrderBy(y => y.DayNo).ToList());
			var schedules = operationResult.Result;
			var doors = new List<ReadOnlyAccessDoorModel>();
			var gkDoors = from cardDoor in cardDoors
						  join gkDoor in GKManager.DeviceConfiguration.Doors on cardDoor.DoorUID equals gkDoor.UID
						  select new { CardDoor = cardDoor, GKDoor = gkDoor };
			foreach (var doorViewModel in gkDoors.Select(x => new ReadOnlyAccessDoorModel(x.GKDoor, x.CardDoor, schedules)).OrderBy(x => x.PresentationName))
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