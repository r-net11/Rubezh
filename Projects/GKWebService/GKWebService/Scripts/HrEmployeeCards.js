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


function EmployeeCardsViewModel(parentViewModel) {
    var self = this;

    self.ParentViewModel = parentViewModel;
    self.IsCardClicked = ko.observable(false);
    self.IsCardSelected = ko.computed(function () {
        return self.IsCardClicked() && self.ParentViewModel.IsRowSelected() && !self.ParentViewModel.IsOrganisation();
    });

    $.ajax({
        dataType: "json",
        url: "Employees/GetEmployeeCards",
        async: false,
        data: null,
        success: function(data) {
            ko.mapping.fromJS(data, {}, self);
        }
    });

    self.InitCards = function (employeeUID) {
        self.IsCardClicked(false);

        if (employeeUID != null) {
            $.getJSON("Employees/GetEmployeeCards/" + employeeUID, function(cards) {
                ko.mapping.fromJS(cards, {}, self);
            });
        }
    };

    self.CanAddCard = ko.computed(function () {
        return !self.ParentViewModel.IsOrganisation();
    }, self);

    self.CardClick = function (data, e, card) {
        $('div.HrCardsPanel li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
        self.IsCardClicked(true);
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
        self.IsCardClicked(false);
    };

    return self;
}