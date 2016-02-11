function HRViewModel(parentViewModel) {
    var self = {};

    self.loaded = ko.observable(false);
    self.ParentViewModel = parentViewModel;

    $.ajax({
        dataType: "json",
        url: "Hr/GetHr",
        async: false,
        data: null,
        success: function (data) {
            ko.mapping.fromJS(data, {}, self);
        }
    });

    self.Init = function () {
        if (!self.loaded()) {
            $("div#HR").load("Hr/Index", function () {
                self.Filter = FilterViewModel(self);
                self.Filter.OrganisationsFilter = OrganisationsFilterViewModel();
                self.Filter.DepartmentsFilter = DepartmentsFilterViewModel();
                self.Filter.PositionsFilter = PositionsFilterViewModel();
                self.Filter.EmployeesFilter = EmployeesFilterViewModel();
                self.Common = {};
                self.Common.EmployeeSelectionDialog = EmployeeSelectionDialogViewModel();
                self.Common.PhotoSelection = PhotoSelectionViewModel();
                self.Employees = EmployeesViewModel(self);
                self.Employees.EmployeeDetails = EmployeeDetailsViewModel(self.Employees);
                self.Employees.EmployeeCardDetails = EmployeeCardDetailsViewModel(self.Employees);
                self.Employees.DepartmentSelection = DepartmentSelectionViewModel();
                self.Employees.PositionSelection = PositionSelectionViewModel();
                self.Employees.ScheduleSelection = ScheduleSelectionViewModel();
                self.Employees.EmployeeCards = EmployeeCardsViewModel(self.Employees);
                self.Employees.CardRemovalReason = CardRemovalReasonViewModel();
                self.Departments = DepartmentsViewModel();
                self.Departments.DepartmentDetails = DepartmentDetailsViewModel(self.Employees.DepartmentSelection);
                self.Departments.DepartmentEmployeeList = DepartmentEmployeeListViewModel(self.Departments);
                self.Positions = PositionsViewModel();
                self.Positions.PositionDetails = PositionDetailsViewModel();
                self.Positions.PositionEmployeeList = PositionEmployeeListViewModel(self.Positions);
                self.Cards = CardsViewModel();
                self.AccessTemplates = AccessTemplatesViewModel();
                self.AccessTemplates.AccessTemplateDetails = AccessTemplateDetailsViewModel();
                self.Organisations = OrganisationsViewModel();
                self.Organisations.OrganisationDetails = OrganisationDetailsViewModel();

                if (self.CanSelectEmployees()) {
                    self.ActivatePage($('div#HR>ul>li#HrPageEmployees'), 'Employees');
                } else if (self.CanSelectDepartments()) {
                    self.ActivatePage($('div#HR>ul>li#HrPageDepartments'), 'Departments');
                } else if (self.CanSelectPositions()) {
                    self.ActivatePage($('div#HR>ul>li#HrPagePositions'), 'Positions');
                } else if (self.CanSelectAdditionalColumns()) {
                    self.ActivatePage($('div#HR>ul>li#HrPageAdditionalColumns'), 'AdditionalColumns');
                } else if (self.CanSelectCards()) {
                    self.ActivatePage($('div#HR>ul>li#HrPageCards'), 'Cards');
                } else if (self.CanSelectAccessTemplates()) {
                    self.ActivatePage($('div#HR>ul>li#HrPageAccessTemplates'), 'AccessTemplates');
                } else if (self.CanSelectPassCardTemplates()) {
                    self.ActivatePage($('div#HR>ul>li#HrPagePassCardTemplates'), 'PassCardTemplates');
                } else if (self.CanSelectOrganisations()) {
                    self.ActivatePage($('div#HR>ul>li#HrPageOrganisations'), 'Organisations');
                }

                ko.applyBindings(app.Menu, $("div#HR")[0]);

                self.loaded(true);

                self.InitializeEmployeeFilter(self.Filter);
            });
        }
    };

    self.hrPages = {
        Employees: ko.observable(false),
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

    self.EmployeesHeader = ko.computed(function () {
        return self.SelectedPersonType() === "Employee" ? "Сотрудники" : "Посетители";
    }, self);

    self.PersonTypeChanged = function (obj, event) {
        self.Filter.UIDs([]);
        self.Filter.PositionUIDs([]);
        self.InitializeEmployeeFilter(self.Filter);
    };

    self.InitializeEmployeeFilter = function(filter) {
        filter.PersonType(self.SelectedPersonType());
        self.Employees.Init(filter);
        self.Departments.Init(self.Filter);
        self.Positions.Init(self.Filter);
        self.Cards.Init(self.Filter);
        self.AccessTemplates.Init(self.Filter);
        self.Organisations.Init(self.Filter);
    };

    self.ActivatePage = function (pageElement, pageName) {
        for (var propertyName in self.hrPages) {
            self.hrPages[propertyName](false);
        }

        self.hrPages[pageName](!self.hrPages[pageName]());
        $('div#HR>ul>li').removeClass("active");
        $(pageElement).addClass("active");
    };

    self.HrPageClick = function(data, e, page) {
        self.ActivatePage($(e.currentTarget).parent(), page);
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