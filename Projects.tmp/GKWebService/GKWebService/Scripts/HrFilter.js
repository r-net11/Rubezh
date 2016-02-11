function FilterViewModel(parentViewModel) {
    var self = {};

    self.HRViewModel = parentViewModel;

    $.ajax({
        dataType: "json",
        url: "/HR/GetFilter/",
        async: false,
        data: null,
        success: function (data) {
            ko.mapping.fromJS(data, {}, self);
        }
    });

    self.filterPages = {
        Organisations: ko.observable(true),
        Departments: ko.observable(false),
        Positions: ko.observable(false),
        Employees: ko.observable(false)
    };

    self.IsWithDeleted = ko.observable(false);
    self.IsShowPositions = ko.observable(false);

    self.Update = function() {
        self.LogicalDeletationType(self.IsWithDeleted() ? "All" : "Active");
    };

    self.IsWithDeleted.subscribe(function(newValue) {
        self.OrganisationsFilter.Init(newValue, self.OrganisationUIDs());
        self.DepartmentsFilter.Init(newValue, self.DepartmentUIDs());
        self.PositionsFilter.Init(self.IsWithDeleted(), self.PositionUIDs());
        self.EmployeesFilter.Init(self.IsWithDeleted(), self.UIDs(), self.LastName(), self.FirstName(), self.SecondName(), self.HRViewModel.SelectedPersonType());
    });

    self.IsNotEmpty = ko.computed(function() {
        return self.FirstName() ||
				self.LastName() ||
				self.SecondName() ||
				self.DepartmentUIDs().length !== 0 ||
				self.PositionUIDs().length !== 0 ||
				self.ScheduleUIDs().length !== 0 ||
				self.OrganisationUIDs().length !== 0 ||
				self.UIDs().length !== 0;
    });

    self.InitFilter = function () {
        self.latestData = ko.mapping.toJS(self);
        self.latestIsWithDeleted = self.IsWithDeleted();
        self.IsShowPositions(self.HRViewModel.SelectedPersonType() === "Employee");
        self.OrganisationsFilter.Init(self.IsWithDeleted(), self.OrganisationUIDs());
        self.DepartmentsFilter.Init(self.IsWithDeleted(), self.DepartmentUIDs());
        self.PositionsFilter.Init(self.IsWithDeleted(), self.PositionUIDs());
        self.EmployeesFilter.Init(self.IsWithDeleted(), self.UIDs(), self.LastName(), self.FirstName(), self.SecondName(), self.HRViewModel.SelectedPersonType());

        self.ActivatePage($('div#filter-box li').first(), 'Organisations');
    };

    self.ActivatePage = function (pageElement, pageName) {
        for (var propertyName in self.filterPages) {
            self.filterPages[propertyName](false);
        }

        self.filterPages[pageName](!self.filterPages[pageName]());
        $('div#filter-box li').removeClass("active");
        $(pageElement).addClass("active");
    };


    self.FilterPageClick = function (data, e, page) {
        self.ActivatePage($(e.currentTarget).parent(), page);
    };

    self.Cancel = function () {
        ko.mapping.fromJS(self.latestData, {}, self);
        self.IsWithDeleted(self.latestIsWithDeleted);
        self.Close();
    };

    self.Close = function () {

        $('#mask , .save-cancel-popup').fadeOut(300, function() {
            $('#mask').remove();
        });

        return false;
    };

    self.Save = function () {
        self.Update();
        self.OrganisationUIDs(self.OrganisationsFilter.GetChecked());
        self.DepartmentUIDs(self.DepartmentsFilter.GetChecked());
        self.PositionUIDs(self.PositionsFilter.GetChecked());
        if (self.EmployeesFilter.IsSearch()) {
            self.LastName(self.EmployeesFilter.LastName());
            self.FirstName(self.EmployeesFilter.FirstName());
            self.SecondName(self.EmployeesFilter.SecondName());
            self.UIDs([]);
        } else {
            self.UIDs(self.EmployeesFilter.GetChecked());
            self.LastName("");
            self.FirstName("");
            self.SecondName("");
        }
        self.HRViewModel.InitializeEmployeeFilter(self);
        self.Close();
    };

    return self;
}
