function EmployeeSelectionDialogViewModel() {
    var self = {};

    self.Employees = ko.observableArray();
    self.SelectedEmployee = ko.observable();

    self.Init = function(okClick) {
        self.OkClick = okClick;
        
        self.SelectedEmployee(null);
    };

    self.EmployeeClick = function (data, e, employee) {
        $('ul#EmployeeSelectionDialogList li').removeClass("selected");
        $(e.currentTarget).parent().addClass("selected");
        self.SelectedEmployee(employee);
    };

    self.Save = function () {
        self.OkClick(self.SelectedEmployee());
        $('#employee-selection-box').fadeOut(300);
    };

    self.Close = function() {
        $('#employee-selection-box').fadeOut(300);
    };

    return self;
}