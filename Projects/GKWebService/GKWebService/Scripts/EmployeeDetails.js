$(document).ready(function () {
    $("#CredentialsStartDate").datepicker();
});

function EmployeeDetailsViewModel() {

    self.LastName = ko.observable("");
    self.FirstName = ko.observable("");
    self.SecondName = ko.observable("");
    self.SaveError = ko.observable("");

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

    return self;
}
