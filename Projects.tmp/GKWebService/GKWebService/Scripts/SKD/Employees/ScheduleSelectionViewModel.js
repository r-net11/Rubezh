$(document).ready(function () {
    $("#ScheduleStartDate").datepicker();
});

function ScheduleSelectionViewModel() {
    var self = {};

    self.Schedules = ko.observableArray();
    self.SelectedSchedule = ko.observable();
    self.ScheduleStartDate = ko.observable();

    self.Init = function (scheduleStartDate, okClick) {
        self.OkClick = okClick;
        self.SelectedSchedule(null);
        self.ScheduleStartDate(scheduleStartDate);
    };

    self.ScheduleClick = function (data, e, schedule) {
        $('ul#ScheduleSelectionList li').removeClass("selected");
        $(e.currentTarget).parent().addClass("selected");
        self.SelectedSchedule(schedule);
    };

    self.Save = function () {
        self.OkClick(self.SelectedSchedule(), self.ScheduleStartDate());
        $('#schedule-selection-box').fadeOut(300, function () {
        });
    };

    self.Close = function() {
        $('#schedule-selection-box').fadeOut(300, function () {
        });
    };

    return self;
}