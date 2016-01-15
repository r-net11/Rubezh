﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GKWebService.DataProviders.SKD;
using RubezhAPI;
using RubezhAPI.SKD;

namespace GKWebService.Models.SKD.AccessTemplates
{
    public class AccessTemplateDetailsViewModel
    {
        public AccessTemplate AccessTemplate { get; set; }

        public List<AccessDoorModel> Doors { get; set; }

        public AccessTemplateDetailsViewModel()
        {
        }

        public void Initialize(Guid? organisationId, Guid? id)
        {
            if (id.HasValue)
            {
                var filter = new AccessTemplateFilter {UIDs = new List<Guid> {id.Value}};
                AccessTemplate = AccessTemplateHelper.Get(filter).Single();
            }
            else
            {
                AccessTemplate = new AccessTemplate()
                {
                    Name = "Новый шаблон доступа",
                };
            }

            var organisation = OrganisationHelper.Get(new OrganisationFilter { UIDs = new List<Guid> { organisationId.Value } }).FirstOrDefault();

            Doors = GKManager.DeviceConfiguration.Doors.Where(door => organisation.DoorUIDs.Any(y => y == door.UID))
                .Select(door => new AccessDoorModel(door, AccessTemplate.CardDoors, GKScheduleHelper.GetSchedules()))
                .ToList();
        }

        public bool Save(bool isNew)
        {
            if (AccessTemplate.Name == "НЕТ")
            {
                throw new InvalidOperationException("Запрещенное название");
            }

            AccessTemplate.CardDoors = Doors.Where(d => d.IsChecked)
                .Select(d => new CardDoor
                {
                    DoorUID = d.DoorUID,
                    EnterScheduleNo = d.SelectedEnterScheduleNo ?? 0,
                    ExitScheduleNo = d.SelectedExitScheduleNo ?? 0,
                    AccessTemplateUID = AccessTemplate.UID
                })
                .ToList();

            var saveResult = AccessTemplateHelper.Save(AccessTemplate, isNew);

            return saveResult;
        }
    }
}