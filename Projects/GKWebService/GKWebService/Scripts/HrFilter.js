function FilterViewModel(parentViewModel) {
    var self = this;

    self.HRViewModel = parentViewModel;

    $.getJSON("/HR/GetFilter/", function (data) {
        ko.mapping.fromJS(data, {}, self);
    });

    self.IsWithDeleted = ko.observable();

    self.InitFilter = function () {
        self.latestData = ko.mapping.toJS(self);
        self.latestIsWithDeleted = self.IsWithDeleted();
    };

    self.Cancel = function() {
        ko.mapping.fromJS(latestData, {}, self);
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
        self.LogicalDeletationType(self.IsWithDeleted() ? "All" : "Active");
        self.HRViewModel.InitializeEmployeeFilter(self);
        self.Close();
    };

    return self;
}
