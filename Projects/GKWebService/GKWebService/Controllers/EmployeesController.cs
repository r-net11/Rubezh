using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Documents;
using GKWebService.DataProviders.SKD;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using RubezhClient;
using GKWebService.Models;
using GKWebService.Models.SKD.Employees;
using GKWebService.Models.SKD.Positions;
using GKWebService.Utils;

namespace GKWebService.Controllers
{
	[Authorize]
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

        [ErrorHandler]
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
        [ErrorHandler]
        public JsonNetResult EmployeeDetails(EmployeeDetailsViewModel employee, bool isNew)
		{
		    var error = employee.Save(isNew);

			return new JsonNetResult { Data = error };
        }

        [HttpPost]
        [ErrorHandler]
		public JsonNetResult SaveChief(SaveChiefParams @params)
        {
			var result = OrganisationHelper.SaveChief(@params.OrganisationUID, @params.EmployeeUID, @params.OrganisationName);

			return new JsonNetResult { Data = result };
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult SaveHRChief(SaveChiefParams @params)
        {
			var result = OrganisationHelper.SaveHRChief(@params.OrganisationUID, @params.EmployeeUID, @params.OrganisationName);

			return new JsonNetResult { Data = result };
		}

        [ErrorHandler]
		public JsonResult GetOrganisations(EmployeeFilter employeeFilter)
        {
            var employeesViewModel = new EmployeesViewModel();
            employeesViewModel.Initialize(employeeFilter);

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = employeesViewModel.Organisations,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ErrorHandler]
        public JsonNetResult GetEmployeePhoto(Guid id)
        {
            var employeeModel = new EmployeeDetailsViewModel();
            employeeModel.Initialize(id);
            return new JsonNetResult { Data = employeeModel.PhotoData ?? string.Empty};
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult MarkDeleted(Guid uid, string name, bool isOrganisation)
		{
			var operationResult = EmployeeHelper.MarkDeleted(uid, name, !isOrganisation);
			return new JsonNetResult { Data = operationResult };
		}

		[HttpPost]
        [ErrorHandler]
        public JsonNetResult Restore(Guid uid, string name, bool isOrganisation)
		{
			var result = EmployeeHelper.Restore(uid, name, !isOrganisation);
			return new JsonNetResult { Data = result };
		}

        [ErrorHandler]
        public JsonNetResult GetOrganisation(Guid? id)
        {
			var filter = new OrganisationFilter();
			filter.UIDs.Add(id.Value);
			var operationResult = OrganisationHelper.Get(filter);
			return new JsonNetResult { Data = operationResult.FirstOrDefault() };
        }

        [ErrorHandler]
        public JsonNetResult GetEmployeeCards(Guid? id)
		{
			var cards = new List<ShortEmployeeCardModel>();
			if (id.HasValue)
			{
				var operationResult = CardHelper.GetByEmployee(id.Value);
				cards.AddRange(operationResult.Select(CreateCard));
			}
			return new JsonNetResult { Data = new {Cards = cards}};
		}

        public ActionResult EmployeeCardDetails()
		{
			return View();
		}

        [ErrorHandler]
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
				card = CardHelper.GetSingle(cardId.Value);
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

			cardModel.Schedules = GKScheduleHelper.GetSchedules();
			cardModel.SelectedScheduleNo = cardModel.Card.GKLevelSchedule;

			var operationResult = CardHelper.Get(new CardFilter { DeactivationType = LogicalDeletationType.Deleted });
			cardModel.StopListCards = operationResult.Where(x => x.IsInStopList).ToList();

			cardModel.AvailableGKControllers = GKManager.Devices.Where(x => x.DriverType == GKDriverType.GK)
																.Select(d =>
																{
																	var isChecked = !cardId.HasValue;
																	isChecked |= card.GKControllerUIDs != null && card.GKControllerUIDs.Contains(d.UID);
																	return new GKControllerModel(d.UID, isChecked, d.PresentationName);
																}).ToList();

			var organisation = OrganisationHelper.Get(new OrganisationFilter {UIDs = new List<Guid>{organisationId.Value}}).FirstOrDefault();

			cardModel.Doors = GKManager.DeviceConfiguration.Doors.Where(door => organisation.DoorUIDs.Any(y => y == door.UID))
				.Select(door => new AccessDoorModel(door, card.CardDoors, cardModel.Schedules))
				.ToList();

			var accessTemplateFilter = new AccessTemplateFilter { OrganisationUIDs = new List<Guid> { organisationId.Value } };
			cardModel.AvailableAccessTemplates = new List<AccessTemplate> { new AccessTemplate { UID = Guid.Empty, Name = "<нет>" } }
				.Concat(AccessTemplateHelper.Get(accessTemplateFilter))
				.ToList();
            var selectedAccessTemplate = cardModel.AvailableAccessTemplates.FirstOrDefault(x => x.UID == cardModel.Card.AccessTemplateUID);
            cardModel.SelectedAccessTemplateId = (selectedAccessTemplate == null ? (Guid?) null : selectedAccessTemplate.UID);

            return new JsonNetResult { Data = cardModel };
		}

		[HttpPost]
        [ErrorHandler]
        public JsonNetResult EmployeeCardDetails(EmployeeCardModel cardModel, string employeeName, bool isNew)
		{
			cardModel.Save();

			bool operationResult;

			if (isNew)
			{
				operationResult = CardHelper.Add(cardModel.Card, employeeName);
			}
			else
			{
				operationResult = CardHelper.Edit(cardModel.Card, employeeName);
			}

			return new JsonNetResult { Data = operationResult };
		}

        public ActionResult CardRemovalReason()
        {
            return View();
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult DeleteCard(Guid id)
        {
            var operationResult = CardHelper.Delete(new SKDCard {UID = id});
            return new JsonNetResult { Data = !operationResult };
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult DeleteFromEmployee(Guid id, string employeeName, string reason)
        {
            var operationResult = CardHelper.DeleteFromEmployee(new SKDCard { UID = id }, employeeName, reason);
            return new JsonNetResult { Data = !operationResult };
        }

        public ActionResult DepartmentSelection()
        {
            return View();
        }

        [ErrorHandler]
        public JsonResult GetDepartments(Guid organisationUID, Guid? departmentUID)
        {
            var operationResult = DepartmentHelper.Get(new DepartmentFilter
            {
                OrganisationUIDs = new List<Guid> { organisationUID },
                ExceptUIDs = (departmentUID.HasValue && departmentUID.Value != Guid.Empty ? new List<Guid> { departmentUID.Value } : new List<Guid> ())
            });

            var departments = new List<DepartmentSelectionItemViewModel>();
            foreach (var rootItem in operationResult.Where(d => d.ParentDepartmentUID == null || d.ParentDepartmentUID == Guid.Empty))
            {
                var itemViewModel = new DepartmentSelectionItemViewModel(rootItem);
                itemViewModel.Level = 0;
                itemViewModel.ParentUID = Guid.Empty;
                itemViewModel.IsLeaf = true;
                departments.Add(itemViewModel);
                int index = departments.IndexOf(itemViewModel);
                AddChildren(departments, itemViewModel, operationResult, ref index);
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

        public ActionResult ScheduleSelection()
        {
            return View();
        }

        [ErrorHandler]
        public JsonNetResult GetSchedules(Guid organisationUID)
        {
            var schedules = ScheduleHelper.GetByOrganisation(organisationUID);

            return new JsonNetResult { Data = new { Schedules = schedules }  };
        }

        public ActionResult PositionSelection()
        {
            return View();
        }

        [ErrorHandler]
        public JsonResult GetPositions(Guid organisationUID, Guid? positionUID)
        {
            var positions = PositionHelper.GetByOrganisation(organisationUID);

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = positions,
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
			employeeCard.Doors = ReadOnlyAccessDoorModel.InitializeDoors(cardDoors);
			return employeeCard;
		}

	    private List<CardDoor> GetCardDoors(SKDCard card)
		{
			var cardDoors = new List<CardDoor>();
			cardDoors.AddRange(card.CardDoors);
			if (card.AccessTemplateUID != null)
			{
				var result = AccessTemplateHelper.Get(new AccessTemplateFilter());
				var accessTemplates = result;
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
	}

	public class SaveChiefParams
	{
		public Guid OrganisationUID { get; set; }
		public Guid? EmployeeUID { get; set; }
		public string OrganisationName { get; set; }
	}
}