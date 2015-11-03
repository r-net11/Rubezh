function HRViewModel(parentViewModel) {
    var self = {};

    self.ParentViewModel = parentViewModel;

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

    self.ParentViewModel.pages["HR"].subscribe(function (newValue) {
        if (newValue) {
            self.Employees.Init();
        }
    });

    self.SelectedPersonType = ko.observable("Employee");

    self.EmployeesHeader = ko.computed(function () {
        return self.SelectedPersonType() == "Employee" ? "Сотрудники" : "Посетители";
    }, self);

    self.PersonTypeChanged = function(obj, event) {
        self.Employees.Init();
    };

    self.HrPageClick = function (data, e, page) {
        for (var propertyName in self.hrPages) {
            self.hrPages[propertyName](false);
        }

        self.hrPages[page](!self.hrPages[page]());
        $('div#HR li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
    }

    return self;
}