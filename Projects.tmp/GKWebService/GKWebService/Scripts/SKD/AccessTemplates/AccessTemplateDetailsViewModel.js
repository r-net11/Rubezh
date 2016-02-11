function AccessTemplateDetailsViewModel() {
    var self = {};

    self.Title = ko.observable();
    self.IsNew = ko.observable(false);
    self.SelectedDoor = ko.observable();

    self.accessTemplateDetailsPages = {
        General: ko.observable(true),
        AccessDoors: ko.observable(false),
    };

    $.ajax({
        dataType: "json",
        url: "AccessTemplates/GetAccessTemplateDetails",
        async: false,
        data: null,
        success: function (data) {
            ko.mapping.fromJS(data, {}, self);
        }
    });

    self.Init = function (organisationUID, uid, okClick) {
        $.getJSON("/AccessTemplates/GetAccessTemplateDetails/",
            { organisationId: organisationUID, id: uid },
            function (data) {
                ko.mapping.fromJS(data, {}, self);
                self.SelectedDoor(null);
                self.OkClick = okClick;
                self.IsNew(false);
                if (uid) {
                    self.Title("Свойства шаблона доступа: " + self.AccessTemplate.Name());
                } else {
                    self.Title("Создание шаблона доступа");
                    self.IsNew(true);
                }
                ShowBox('#accessTemplate-details-box');
        })
        .fail(function (jqxhr, textStatus, error) {
            ShowError(jqxhr.responseText);
        });
    };

    self.AccessTemplateDetailsPageClick = function (data, e, page) {
        for (var propertyName in self.accessTemplateDetailsPages) {
            self.accessTemplateDetailsPages[propertyName](false);
        }

        self.accessTemplateDetailsPages[page](!self.accessTemplateDetailsPages[page]());
        $('div#accessTemplate-details-box li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
    };

    self.AccessTemplateDetailsClose = function () {
        CloseBox();
    };

    self.SaveAccessTemplate = function () {
        if (self.SelectedDoor()) {
            self.SelectedDoor().SelectedEnterScheduleNo(self.SelectedDoor().SelectedEnterSchedule().ScheduleNo());
            self.SelectedDoor().SelectedExitScheduleNo(self.SelectedDoor().SelectedExitSchedule().ScheduleNo());
        }

        var data = ko.mapping.toJSON(self);
        $.ajax({
            url: "AccessTemplates/AccessTemplateDetails",
            type: "post",
            contentType: "application/json",
            data: "{'accessTemplate':" + data + ",'isNew': '" + self.IsNew() + "'}",
            success: function (error) {
                if (self.OkClick) {
                    self.OkClick();
                }
                CloseBox();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowError(xhr.responseText);
            }
        });
    };

    self.DoorClick = function (data, e, door) {
        $('ul#AccessTemplateDetailsDoors li').removeClass("selected");
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

    return self;
}