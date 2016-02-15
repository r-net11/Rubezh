function EmployeeCardDetailsViewModel(parentViewModel) {
    var self = {};

    $.ajax({
        dataType: "json",
        url: "Employees/GetEmployeeCardDetails",
        async: false,
        data: null,
        success: function (data) {
            ko.mapping.fromJS(data, {}, self);
        }
    });

    self.Title = ko.observable();
    self.ParentViewModel = parentViewModel;
    self.CanChangeCardType = ko.observable();
    self.SelectedGCCardType = ko.observable();
    self.SelectedStopListCard = ko.observable();
    self.CanSelectGKControllers = ko.computed(function () {
        return self.SelectedGCCardType() != 0;
    }, self);

    self.UseReader = ko.observable(false);

    self.IsNewCard = ko.observable();

    self.AccessDoorIsChecked = ko.observable();

    self.employeeCardDetailsPages = {
        General: ko.observable(true),
        GKControllers: ko.observable(false),
        AccessDoors: ko.observable(false),
        AccessTemplates: ko.observable(false),
    };

    self.InitEmployeeCardDetails = function (organisationUID, cardId, isNew) {
        self.ActivatePage($('div#employee-card-details-box ul.tab li').first(), 'General');
        self.OrganisationUID = organisationUID;
        var sendData = "{'organisationId':'" + organisationUID + "','cardId': '" + cardId + "'}";
        $.ajax({
            url: "Employees/GetEmployeeCardDetails",
            type: "post",
            contentType: "application/json",
            data: sendData,
            success: function (data) {
                ko.mapping.fromJS(data, {}, self);
                self.IsNewCard(isNew);
                if (isNew) {
                    self.Title("Создание пропуска");
                } else {
                    self.Title(("Свойства пропуска: ") + self.Card.Number());
                }

                self.IsGuest(self.ParentViewModel.IsGuest());
                self.CanChangeCardType(!self.IsGuest() && app.Menu.HR.IsEmployeesEditCardTypeAllowed());
                self.SelectedGCCardType(self.Card.GKCardType());

                if (self.SelectedScheduleNo()) {
                    var foundSchedule = ko.utils.arrayFirst(self.Schedules(), function(s) {
                        return s.No() === self.SelectedScheduleNo();
                    });
                    if (foundSchedule) {
                        self.SelectedSchedule(foundSchedule);
                    }
                }

                if (self.SelectedAccessTemplateId()) {
                    var selectItem = ko.utils.arrayFirst($('ul#EmployeeCardDetailsTemplates li'), function(element) {
                        return ko.dataFor(element).UID() === self.SelectedAccessTemplateId();
                    });
                    $(selectItem).addClass("selected");
                } else {
                    
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowError(xhr.responseText);
            }
        });
    };

    self.ActivatePage = function(pageElement, pageName) {
        for (var propertyName in self.employeeCardDetailsPages) {
            self.employeeCardDetailsPages[propertyName](false);
        }

        self.employeeCardDetailsPages[pageName](!self.employeeCardDetailsPages[pageName]());
        $('div#employee-card-details-box ul.tab li').removeClass("active");
        $(pageElement).addClass("active");
    };


    self.EmployeeCardDetailsPageClick = function (data, e, page) {
        self.ActivatePage($(e.currentTarget).parent(), page);
    };


    self.UpdateStopListCard = function() {
        if (self.UseStopList() && self.SelectedStopListCard() != null) {
            self.Card.Number(self.SelectedStopListCard().Number());
        }
    };

    self.UseStopList.subscribe(self.UpdateStopListCard);
    self.SelectedStopListCard.subscribe(self.UpdateStopListCard);

    self.EmployeeCardDetailsClose = function () {
        $('#mask , .save-cancel-popup').fadeOut(300, function () {
            $('#mask').remove();
        });

        return false;
    };

    self.Save = function() {
        self.Card.OrganisationUID(self.OrganisationUID);
        self.Card.EmployeeUID(self.ParentViewModel.UID());
        self.Card.EmployeeName(self.ParentViewModel.Name());
        self.Card.GKCardType(self.SelectedGCCardType());

        if (self.SelectedSchedule()) {
            self.SelectedScheduleNo(self.SelectedSchedule().No());
        }

        if (self.SelectedDoor()) {
            self.SelectedDoor().SelectedEnterScheduleNo(self.SelectedDoor().SelectedEnterSchedule().ScheduleNo());
            self.SelectedDoor().SelectedExitScheduleNo(self.SelectedDoor().SelectedExitSchedule().ScheduleNo());
        }

        var mapping = {
            'ignore': ["Schedules", "StopListCards", "AvailableAccessTemplates", "SelectedDoor"]
        };
        var data = ko.mapping.toJSON(self, mapping);
        $.ajax({
            url: "Employees/EmployeeCardDetails",
            type: "post",
            contentType: "application/json",
            data: "{'cardModel':" + data + ",'employeeName': '" + self.ParentViewModel.Name() + "','isNew': '" + self.IsNewCard() + "'}",
            success: function() {
                self.EmployeeCardDetailsClose();
                self.ParentViewModel.EmployeeCards.ReloadCards();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowError(xhr.responseText);
            }
        });
    };

    self.SaveEmployeeCard = function () {
        var stopListCard = ko.utils.arrayFirst(self.StopListCards(), function(item) {
            return item.Number() === self.Card.Number();
        });

        if (stopListCard) {
            if (confirm("Карта с таким номером находится в стоп-листе. Использовать её?"))
            {
                self.UseStopList(true);
                self.SelectedStopListCardId(stopListCard.UID());
                self.Save();
            } else {
                return;
            }
        } else {
            self.Save();
        }
    };

    self.ChangeReaderClick = function() {

    };

    self.ShowUSBCardReaderClick = function () {

    };

    self.DoorClick = function (data, e, door) {
        $('ul#EmployeeCardDetailsDoors li').removeClass("selected");
        $(e.currentTarget).parent().addClass("selected");
        if (self.SelectedDoor()) {
            self.SelectedDoor().SelectedEnterScheduleNo(self.SelectedDoor().SelectedEnterSchedule().ScheduleNo());
            self.SelectedDoor().SelectedExitScheduleNo(self.SelectedDoor().SelectedExitSchedule().ScheduleNo());
        }
        self.SelectedDoor(door);
        if (door.SelectedEnterScheduleNo()) {
            var foundSchedule = ko.utils.arrayFirst(door.EnterSchedules(), function (schedule) {
                return schedule.ScheduleNo() === door.SelectedEnterScheduleNo();
            });
            if (foundSchedule) {
                door.SelectedEnterSchedule(foundSchedule);
            }
        };
        if (door.SelectedExitScheduleNo()) {
            var foundExitSchedule = ko.utils.arrayFirst(door.ExitSchedules(), function (schedule) {
                return schedule.ScheduleNo() === door.SelectedExitScheduleNo();
            });
            if (foundExitSchedule) {
                door.SelectedExitSchedule(foundExitSchedule);
            }
        };
        return false;
    };

    self.AvailableAccessTemplateClick = function(data, e, template) {
        $('ul#EmployeeCardDetailsTemplates li').removeClass("selected");
        $(e.currentTarget).parent().addClass("selected");
        self.SelectedAccessTemplateId(template.UID());
    };

    return self;
}
