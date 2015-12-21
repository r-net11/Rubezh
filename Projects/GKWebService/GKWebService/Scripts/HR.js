function HRViewModel(parentViewModel) {
    var self = {};

    self.loaded = ko.observable(false);
    self.ParentViewModel = parentViewModel;

    self.Init = function () {
        if (!self.loaded()) {
            $("div#HR").load("Hr/Index", function () {
                self.Filter = FilterViewModel(self);
                self.Filter.OrganisationsFilter = OrganisationsFilterViewModel(self);
                self.Filter.DepartmentsFilter = DepartmentsFilterViewModel(self);
                self.Filter.PositionsFilter = PositionsFilterViewModel(self);
                self.Filter.EmployeesFilter = EmployeesFilterViewModel(self);
                self.Common = {};
                self.Common.EmployeeSelectionDialog = EmployeeSelectionDialogViewModel();
                self.Employees = EmployeesViewModel(self);
                self.Employees.EmployeeDetails = EmployeeDetailsViewModel();
                self.Employees.EmployeeCardDetails = EmployeeCardDetailsViewModel(self.Employees);
                self.Employees.DepartmentSelection = DepartmentSelectionViewModel();
                self.Employees.EmployeeCards = EmployeeCardsViewModel(self.Employees);
                self.Employees.CardRemovalReason = CardRemovalReasonViewModel();
                self.Departments = DepartmentsViewModel();
                self.Departments.DepartmentDetails = DepartmentDetailsViewModel(self.Employees.DepartmentSelection);
                self.Departments.DepartmentEmployeeList = DepartmentEmployeeListViewModel(self.Departments);
                self.Positions = PositionsViewModel();
                self.Positions.PositionDetails = PositionDetailsViewModel();
                self.Positions.PositionEmployeeList = PositionEmployeeListViewModel(self.Positions);
                self.Organisations = OrganisationsViewModel();
                self.Organisations.OrganisationDetails = OrganisationDetailsViewModel();
                ko.applyBindings(app.Menu, $("div#HR")[0]);
                self.loaded(true);
                self.InitializeEmployeeFilter(self.Filter);
            });
        }
    };

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
            self.Init();
        }
    });

    self.SelectedPersonType = ko.observable("Employee");

    self.EmployeesHeader = ko.computed(function () {
        return self.SelectedPersonType() == "Employee" ? "Сотрудники" : "Посетители";
    }, self);

    self.PersonTypeChanged = function(obj, event) {
        self.InitializeEmployeeFilter(self.Filter);
    };

    self.InitializeEmployeeFilter = function(filter) {
        filter.PersonType(self.SelectedPersonType());
        self.Employees.Init(filter);
        self.Departments.Init(self.Filter);
        self.Positions.Init(self.Filter);
        self.Organisations.Init(self.Filter);
    };

    self.HrPageClick = function(data, e, page) {
        for (var propertyName in self.hrPages) {
            self.hrPages[propertyName](false);
        }

        self.hrPages[page](!self.hrPages[page]());
        $('div#HR li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
    };

    self.EditFilter = function (data, e, box) {
        self.Filter.InitFilter();
        ShowBox(box);
    };

    self.ChangeIsDeleted = function(data, e) {
        self.Filter.IsWithDeleted(!self.Filter.IsWithDeleted());
        self.Filter.Update();
        self.InitializeEmployeeFilter(self.Filter);
    };

    return self;
}