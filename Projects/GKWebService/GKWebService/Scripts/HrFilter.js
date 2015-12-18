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

    self.IsWithDeleted = ko.observable(false);

    self.Update = function() {
        self.LogicalDeletationType(self.IsWithDeleted() ? "All" : "Active");
    };

    self.InitFilter = function () {
        self.latestData = ko.mapping.toJS(self);
        self.latestIsWithDeleted = self.IsWithDeleted();
    };

    self.Cancel = function() {
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
