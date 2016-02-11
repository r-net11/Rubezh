$(document).ready(function () {
    $("#CredentialsStartDate").datepicker();
    $("#BirthDate").datepicker();
    $("#GivenDate").datepicker();
    $("#ValidTo").datepicker();
});

function EmployeeDetailsViewModel(parentViewModel) {
    var self = {};

    self.EmptyGuid = '00000000-0000-0000-0000-000000000000';
    self.ParentViewModel = parentViewModel;
    self.Title = ko.observable();
    self.IsEmployee = ko.observable(true);

    self.employeeDetailsPages = {
        General: ko.observable(true),
        Document: ko.observable(false),
        Photo: ko.observable(false),
    };

    self.IsOrganisationChief = ko.observable();
    self.IsOrganisationHRChief = ko.observable();

    $.ajax({
        dataType: "json",
        url: "Employees/GetEmployeeDetails",
        async: false,
        data: null,
        success: function (data) {
            ko.mapping.fromJS(data, {}, self);
        }
    });

    self.IsPositionSelected = ko.computed(function () {
        return self.Employee.PositionUID() !== self.EmptyGuid;
    }, self);

    self.IsDepartmentSelected = ko.computed(function () {
        return self.Employee.DepartmentUID() !== self.EmptyGuid;
    }, self);

    self.IsEscortSelected = ko.computed(function () {
        return self.Employee.EscortUID() && self.Employee.EscortUID() !== self.EmptyGuid;
    }, self);

    self.IsScheduleSelected = ko.computed(function () {
        return self.Employee.ScheduleUID() && self.Employee.ScheduleUID() !== self.EmptyGuid;
    }, self);

    self.ScheduleName = ko.computed(function () {
        var date = $.datepicker.parseDate("yy-mm-dd", self.Employee.ScheduleStartDate().split("T")[0]);
        return self.Employee.ScheduleName() + ' с ' + $.datepicker.formatDate("dd.mm.yy", date);
    }, self);

    self.FIO = function () {
        var names = [self.Employee.LastName(), self.Employee.FirstName(), self.Employee.SecondName()];
        return names.join(" ");
    };

    self.Init = function (isNew, personType, okClick) {
        self.IsNew = isNew;
        self.IsEmployee(personType === "Employee");
        self.Employee.Type(personType);
        self.OkClick = okClick;
        if (isNew) {
            self.Title(self.IsEmployee() ? "Добавить сотрудника" : "Добавить посетителя");
            self.Employee.OrganisationUID(self.Organisation.UID);
            self.IsOrganisationChief(false);
            self.IsOrganisationHRChief(false);
        } else {
            self.Title((self.IsEmployee() ? "Свойства сотрудника: " : "Свойства посетителя: ") + self.FIO());
            self.IsOrganisationChief(self.Organisation.ChiefUID == self.Employee.UID());
            self.IsOrganisationHRChief(self.Organisation.HRChiefUID == self.Employee.UID());
        }
    };

    self.SaveError = ko.observable("");

    self.EmployeeDetailsPageClick = function(data, e, page) {
        for (var propertyName in self.employeeDetailsPages) {
            self.employeeDetailsPages[propertyName](false);
        }

        self.employeeDetailsPages[page](!self.employeeDetailsPages[page]());
        $('div#employee-details-box li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
    };

    self.EmployeeDetailsClose = function() {
        $('#mask , .save-cancel-popup').fadeOut(300, function() {
            $('#mask').remove();
        });

        return false;
    };

    self.SaveEmployee = function() {
        var data = ko.mapping.toJSON(self);
        $.ajax({
            url: "Employees/EmployeeDetails",
            type: "post",
            contentType: "application/json",
            data: "{'employee':" + data + ",'isNew': '" + self.IsNew + "'}",
            success: function () {
                self.SaveChief(self.IsOrganisationChief(), 
                    self.Organisation.ChiefUID, 
                    "Employees/SaveChief", 
                    function() {
                        self.SaveChief(self.IsOrganisationHRChief(),
                            self.Organisation.HRChiefUID,
                            "Employees/SaveHRChief",
                            function () {
                                self.OkClick();
                                self.EmployeeDetailsClose();
                            });
                    });
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowError(xhr.responseText);
            },
        });
    };

    self.SaveChief = function (isOrganisationChief, organisationChiefUID, url, success) {
        var uid;
        if (isOrganisationChief && organisationChiefUID != self.Employee.UID()) {
            uid = self.UID();
        } else if (!isOrganisationChief && organisationChiefUID == self.Employee.UID()) {
            uid = "";
        }
        if (uid != undefined) {
            $.ajax({
                url: url,
                type: "post",
                contentType: "application/json",
                data: "{ 'OrganisationUID': '" +
                    self.Organisation.UID +
                    "', 'EmployeeUID': '" +
                    uid +
                    "', 'OrganisationName': '" +
                    self.Organisation.Name +
                    "'}",
                success: function() {
                    success();
                },
                error: function(xhr, ajaxOptions, thrownError) {
                    ShowError(xhr.responseText);
                },
            });
        } else {
            success();
        }
    };

    self.SelectPosition = function () {
        self.ParentViewModel.PositionSelection.Init(self.Employee.OrganisationUID(), self.Employee.UID(), function (uid, name) {
            if (uid) {
                self.Employee.PositionUID(uid);
                self.Employee.PositionName(name);
            } else {
                self.Employee.PositionUID(self.EmptyGuid);
                self.Employee.PositionName(null);
            }
        });
    };

    self.SelectDepartment = function() {
        self.ParentViewModel.DepartmentSelection.Init(self.Employee.OrganisationUID(), null, function (uid, name) {
            if (uid) {
                self.Employee.DepartmentUID(uid);
                self.Employee.DepartmentName(name);
            } else {
                self.Employee.DepartmentUID(self.EmptyGuid);
                self.Employee.DepartmentName(null);
            }
        });
    };

    self.SelectEscort = function() {
        $.getJSON("/Hr/GetOrganisationDepartmentEmployees/",
            {'organisationId': self.Employee.OrganisationUID() , 'departmentId': self.Employee.DepartmentUID() },
            function (emp) {
                ko.mapping.fromJS(emp, {}, app.Menu.HR.Common.EmployeeSelectionDialog);
                app.Menu.HR.Common.EmployeeSelectionDialog.Init(function (employee) {
                    if (employee) {
                        self.Employee.EscortUID(employee.UID());
                        self.Employee.EscortName(employee.Name());
                    } else {
                        self.Employee.EscortUID(self.EmptyGuid);
                        self.Employee.EscortName(null);
                    }
                });
            ShowBox('#employee-selection-box');
        })
        .fail(function (jqxhr, textStatus, error) {
            ShowError(jqxhr.responseText);
        });
    };

    self.SelectSchedule = function() {
        $.getJSON("/Employees/GetSchedules/",
            { 'organisationUID': self.Employee.OrganisationUID() },
            function (emp) {
                ko.mapping.fromJS(emp, {}, app.Menu.HR.Employees.ScheduleSelection);
                app.Menu.HR.Employees.ScheduleSelection.Init(self.Employee.ScheduleStartDate(), function (schedule, scheduleStartDate) {
                    if (schedule) {
                        self.Employee.ScheduleUID(schedule.UID());
                        self.Employee.ScheduleName(schedule.Name());
                    } else {
                        self.Employee.ScheduleUID(self.EmptyGuid);
                        self.Employee.ScheduleName(null);
                    }
                    self.Employee.ScheduleStartDate(scheduleStartDate);
                });
                ShowBox('#schedule-selection-box');
            })
        .fail(function (jqxhr, textStatus, error) {
            ShowError(jqxhr.responseText);
        });
    };

    return self;
}
