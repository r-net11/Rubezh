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
        var data = ko.mapping.toJSON(self);
        $.ajax({
            url: location.href,
            type: "post",
            contentType: "application/json",
            data: data,
            success: function (response) {
                alert(response.Status);
            }
        });
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
