using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.Models
{
	public class EmployeeCardModel
	{
		public SKDCard Card { get; set; }
		
		public List<GKSchedule> Schedules { get; set; }
		
		public GKSchedule SelectedSchedule { get; set; }

		public int? SelectedScheduleNo { get; set; }

		public bool UseStopList { get; set; }

		public List<SKDCard> StopListCards { get; set; }

		public SKDCard SelectedStopListCard { get; set; }

		public Guid? SelectedStopListCardId { get; set; }

		public List<GKControllerModel> AvailableGKControllers { get; set; }

		public List<AccessDoorModel> Doors { get; set; }

		public AccessDoorModel SelectedDoor { get; set; }

		public List<AccessTemplate> AvailableAccessTemplates { get; set; }

		public Guid? SelectedAccessTemplateId { get; set; }

		public bool IsGuest { get; set; }
		
		public EmployeeCardModel()
		{
		}

		public void Save()
		{
			if (UseStopList && SelectedStopListCardId.HasValue)
			{
				Card.UID = SelectedStopListCardId.Value;
				Card.IsInStopList = false;
				Card.StopReason = null;
			}
			if (IsGuest)
				Card.GKCardType = GKCardType.Employee;

			if (Card.GKCardType != GKCardType.Employee)
			{
				Card.GKControllerUIDs = AvailableGKControllers.Where(x => x.IsChecked).Select(x => x.UID).ToList();
			}

			if (SelectedScheduleNo.HasValue)
			{
				Card.GKLevelSchedule = SelectedScheduleNo.Value;
			}

			Card.CardDoors = Doors.Where(d => d.IsChecked)
				.Select(d => new CardDoor
				{
					DoorUID = d.DoorUID,
					EnterScheduleNo = d.SelectedEnterScheduleNo ?? 0,
					ExitScheduleNo = d.SelectedExitScheduleNo ?? 0,
					CardUID = Card.UID,
				})
				.ToList();

			if (SelectedAccessTemplateId != null)
			{
				if (SelectedAccessTemplateId.Equals(Guid.Empty))
				{
					Card.AccessTemplateUID = null;
				}
				else
				{
					Card.AccessTemplateUID = SelectedAccessTemplateId;
				}
			}

			var error = Validate();
			if (!string.IsNullOrEmpty(error))
			{
				throw new InvalidOperationException(error);
			}
		}
		private string Validate()
		{
			if (Card.Number <= 0 || Card.Number > Int32.MaxValue)
			{
				return "Номер карты должен быть задан в пределах 1 ... 2147483647";
			}

			if (Card.GKLevel < 0 || Card.GKLevel > 255)
			{
				return "Уровень доступа должен быть в пределах от 0 до 255";
			}
			
			return string.Empty;
		}
	}
}