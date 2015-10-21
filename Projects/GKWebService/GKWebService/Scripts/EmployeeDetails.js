$(document).ready(function () {
    $("#CredentialsStartDate").datepicker();
    $("#BirthDate").datepicker();
    $("#GivenDate").datepicker();
    $("#ValidTo").datepicker();
});

function EmployeeDetailsViewModel() {
    var self = this;

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

    $('#employee-details-box').on('fadeInComplete', function () {
    });

    self.Init = function (isNew) {
        self.IsNew = isNew;
        if (isNew) {
            self.OrganisationUID(self.Organisation.UID);
            self.IsOrganisationChief(false);
            self.IsOrganisationHRChief(false);
        } else {
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
            success: function (response) {
                self.SaveChief(self.IsOrganisationChief(), self.Organisation.ChiefUID, "Employees/SaveChief");
                self.SaveChief(self.IsOrganisationHRChief(), self.Organisation.HRChiefUID, "Employees/SaveHRChief");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("request failed");
            },
        });

        EmployeeDetailsClose();
    };

    self.SaveChief = function (isOrganisationChief, organisationChiefUID, url) {
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
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("request failed");
                },
            });
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
