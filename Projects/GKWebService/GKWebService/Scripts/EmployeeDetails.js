$(document).ready(function () {
    $("#CredentialsStartDate").datepicker();
    $("#BirthDate").datepicker();
    $("#GivenDate").datepicker();
    $("#ValidTo").datepicker();
});

function EmployeeDetailsViewModel() {

    self.employeeDetailsPages = {
        General: ko.observable(true),
        Document: ko.observable(false),
        Photo: ko.observable(false),
    };

    self.EmployeeDetailsModel = {
        LastName: ko.observable(""),
        FirstName: ko.observable(""),
        SecondName: ko.observable(""),
        Description: ko.observable(""),
        CredentialsStartDateString: ko.observable(""),
        TabelNo: ko.observable(""),
        Phone: ko.observable(""),
        IsOrganisationChief: ko.observable(""),
        IsOrganisationHRChief: ko.observable(""),
        DocumentNumber: ko.observable(""),
        BirthPlace: ko.observable(""),
        BirthDateString: ko.observable(""),
        GivenBy: ko.observable(""),
        GivenDateString: ko.observable(""),
        ValidToString: ko.observable(""),
        Citizenship: ko.observable(""),
        Gender: ko.observable(""),
        DocumentType: ko.observable(""),
    };

    self.SaveError = ko.observable("");

    self.EmployeeDetailsPageClick = function (data, e, page) {
        for (var propertyName in self.employeeDetailsPages) {
            self.employeeDetailsPages[propertyName](false);
        }

        self.employeeDetailsPages[page](!self.employeeDetailsPages[page]());
        $('div#employee-details-box li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
    }

    self.EmployeeDetailsClose = function () {
        $('#mask , .save-cancel-popup').fadeOut(300, function () {
            $('#mask').remove();
        });

        return false;
    }

    self.SaveEmployee = function () {
        EmployeeDetailsClose();
    }

    self.SelectPosition = function () {
        EmployeeDetailsClose();
    }

    self.SelectDepartment = function () {
        EmployeeDetailsClose();
    }

    self.SelectEscort = function () {
        EmployeeDetailsClose();
    }

    self.SelectSchedule = function () {
        EmployeeDetailsClose();
    }

    return self;
}
