function HRViewModel() {
    var self = {};

    self.hrPages = {
        Employees: ko.observable(true),
        Departments: ko.observable(false),
        Positions: ko.observable(false),
        AdditionalColumnTypes: ko.observable(false),
        Cards: ko.observable(false),
        AccessTemplates: ko.observable(false),
        PassCardTemplates: ko.observable(false),
        Organisations: ko.observable(false)
    };

    self.SelectedPersonType = ko.observable("Employee");

    self.EmployeesHeader = ko.computed(function () {
        return self.SelectedPersonType() == "Employee" ? "Сотрудники" : "Посетители";
    }, self);

    self.HrPageClick = function (data, e, page) {
        for (var propertyName in self.hrPages) {
            self.hrPages[propertyName](false);
        }

        self.hrPages[page](!self.hrPages[page]());
        $('div#HR li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
    }

    self.AddEmployeeClick = function (data, e, box) {
        //Fade in the Popup
        $(box).fadeIn(300);

        //Set the center alignment padding + border see css style
        var popMargTop = ($(box).height() + 24) / 2;
        var popMargLeft = ($(box).width() + 24) / 2;

        $(box).css({
            'margin-top': -popMargTop,
            'margin-left': -popMargLeft
        });

        // Add the mask to body
        $('body').append('<div id="mask"></div>');
        $('#mask').fadeIn(300);

        return false;
    }

    return self;
}

