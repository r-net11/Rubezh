function HRViewModel() {
    var self = {};

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

