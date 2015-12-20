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

    self.Update = function() {
        self.LogicalDeletationType(self.IsWithDeleted() ? "All" : "Active");
    };

    self.IsWithDeleted.subscribe(function(newValue) {
        self.OrganisationsFilter.Init(newValue);
    });

    self.InitFilter = function () {
        self.latestData = ko.mapping.toJS(self);
        self.latestIsWithDeleted = self.IsWithDeleted();
        self.OrganisationsFilter.Init(self.IsWithDeleted());
    };

    self.FilterPageClick = function (data, e, page) {
        for (var propertyName in self.filterPages) {
            self.filterPages[propertyName](false);
        }

        self.filterPages[page](!self.filterPages[page]());
        $('div#filter-box li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
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
        self.HRViewModel.InitializeEmployeeFilter(self);
        self.Close();
    };

    return self;
}
