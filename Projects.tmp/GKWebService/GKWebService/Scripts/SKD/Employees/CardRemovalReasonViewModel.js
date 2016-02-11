function CardRemovalReasonViewModel() {
    var self = {};

    self.RemoveType = ko.observable();
    self.RemovalReason = ko.observable();
    self.RemoveIsChecked = ko.computed(function() {
        return self.RemoveType() === "Remove";
    }, self);
    self.DeactivateIsChecked = ko.computed(function() {
        return self.RemoveType() === "Deactivate";
    }, self);
    self.CanSave = ko.computed(function() {
        return !self.DeactivateIsChecked() || self.RemovalReason();
    }, self);

    self.Init = function(save) {
        self.Save = save;
        self.RemoveType("Remove");
        self.RemovalReason("Утерян " + $.datepicker.formatDate('dd.mm.yy', new Date()));
    };

    self.OkClick = function() {
        self.Save();
    };

    self.Close = function () {
        CloseBox();
    };

    return self;
}