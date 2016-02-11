function EmployeeSelectionDialogViewModel() {
    var self = {};

    self.Employees = ko.observableArray();
    self.SelectedEmployee = ko.observable();

    self.Init = function(okClick, isRootModal) {
        self.OkClick = okClick;
        self.IsRootModal = isRootModal;
        self.SelectedEmployee(null);
    };

    self.EmployeeClick = function (data, e, employee) {
        $('ul#EmployeeSelectionDialogList li').removeClass("selected");
        $(e.currentTarget).parent().addClass("selected");
        self.SelectedEmployee(employee);
    };

    self.Save = function () {
        self.OkClick(self.SelectedEmployee());
        $('#employee-selection-box').fadeOut(300, function () {
            if (self.IsRootModal) {
                $('#mask').remove();
            }
        });
    };

    self.Close = function() {
        $('#employee-selection-box').fadeOut(300, function () {
            if (self.IsRootModal) {
                $('#mask').remove();
            }
        });
    };

    return self;
}