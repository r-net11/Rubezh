$(document).ready(function () {
    $("#jqGridDoors").jqGrid({
        datastr: null,
        datatype: "jsonstring",
        colModel: [
            { label: 'Точка доступа', name: 'PresentationName', sortable: false },
            { label: 'Вход', name: 'EnerScheduleName', sortable: false },
            { label: 'Выход', name: 'ExitScheduleName', sortable: false }
        ],
        width: 990,
        height: 200,
        rowNum: 100,
        viewrecords: true,
    });
});


function EmployeeCardsViewModel() {
    var self = this;

    self.IsCardSelected = ko.observable(false);

    $.ajax({
        dataType: "json",
        url: "Employees/GetEmployeeCards",
        async: false,
        data: null,
        success: function(data) {
            ko.mapping.fromJS(data, {}, self);
        }
    });

    self.Init = function (employeeUID) {
        self.IsCardSelected(false);
        $.getJSON("Employees/GetEmployeeCards/" + employeeUID, function (cards) {
            ko.mapping.fromJS(cards, {}, self);
        });
    };

    self.CardClick = function (data, e, card) {
        $('div.HrCardsPanel li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
        self.IsCardSelected(true);
        $("#jqGridDoors").setGridParam({
            datastr: ko.toJSON(card.Doors),
            datatype: "jsonstring",
            treedatatype: "jsonstring",
        });
        $("#jqGridDoors").trigger("reloadGrid");
    };

    self.EmployeeClick = function (data, e) {
        $('div.HrCardsPanel li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
        self.IsCardSelected(false);
    };

    return self;
}