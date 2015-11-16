$(document).ready(function () {
    $("#CredentialsStartDate").datepicker();
    $("#BirthDate").datepicker();
    $("#GivenDate").datepicker();
    $("#ValidTo").datepicker();
});

function EmployeeDetailsViewModel() {
    var self = this;

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

    self.FIO = function () {
        var names = [self.LastName(), self.FirstName(), self.SecondName()];
        return names.join(" ");
    };

    self.Init = function (isNew, personType, parentViewModel) {
        self.IsNew = isNew;
        self.IsEmployee(personType === "Employee");
        self.Type(personType);
        self.ParentViewModel = parentViewModel;
        if (isNew) {
            self.Title(self.IsEmployee() ? "Добавить сотрудника" : "Добавить посетителя");
            self.OrganisationUID(self.Organisation.UID);
            self.IsOrganisationChief(false);
            self.IsOrganisationHRChief(false);
        } else {
            self.Title((self.IsEmployee() ? "Свойства сотрудника: " : "Свойства посетителя: ") + self.FIO());
            self.IsOrganisationChief(self.Organisation.ChiefUID == UID());
            self.IsOrganisationHRChief(self.Organisation.HRChiefUID == UID());
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
                                self.ParentViewModel.ReloadTree();
                                EmployeeDetailsClose();
                            });
                    });
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("request failed");
            },
        });
    };

    self.SaveChief = function (isOrganisationChief, organisationChiefUID, url, success) {
        var uid;
        if (isOrganisationChief && organisationChiefUID != self.UID()) {
            uid = self.UID();
        } else if (!isOrganisationChief && organisationChiefUID == self.UID()) {
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
                    alert("request failed");
                },
            });
        } else {
            success();
        }
    };

    self.SelectPosition = function() {
        EmployeeDetailsClose();
    };

    self.SelectDepartment = function() {
        EmployeeDetailsClose();
    };

    self.SelectEscort = function() {
        EmployeeDetailsClose();
    };

    self.SelectSchedule = function() {
        EmployeeDetailsClose();
    };

    return self;
}
