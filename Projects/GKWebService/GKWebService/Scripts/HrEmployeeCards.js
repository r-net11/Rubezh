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
    var self = {};

    self.EmployeesParentViewModel = parentViewModel;
    self.IsCardClicked = ko.observable(false);
    self.IsCardSelected = ko.computed(function () {
        return self.IsCardClicked() && self.EmployeesParentViewModel.IsRowSelected() && !self.EmployeesParentViewModel.IsOrganisation();
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
        return !self.EmployeesParentViewModel.IsOrganisation() && !self.EmployeesParentViewModel.IsDeleted();
    }, self);

    self.CanEditCard = ko.computed(function () {
        return true;
    }, self);

    self.CardClick = function (data, e, card) {
        $('div.HrCardsPanel li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
        self.IsCardClicked(true);
        self.Card = card;
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

    self.EditEmployeeCardClick = function (data, e, box) {
        $.getJSON("/Employees/GetEmployeeCardDetails/" + self.Card.UID(), function (card) {
            ko.mapping.fromJS(card, {}, self.EmployeesParentViewModel.EmployeeCardDetails);
            self.EmployeesParentViewModel.EmployeeCardDetails.InitEmployeeCardDetails(false);
            ShowBox(box);
        });
    };

    self.AddEmployeeCardClick = function (data, e, box) {
        $.getJSON("/Employees/GetEmployeeCardDetails/", function (card) {
            ko.mapping.fromJS(card, {}, self.EmployeesParentViewModel.EmployeeCardDetails);
            self.EmployeesParentViewModel.EmployeeCardDetails.InitEmployeeCardDetails(true);
            ShowBox(box);
        });
    };

    return self;
}