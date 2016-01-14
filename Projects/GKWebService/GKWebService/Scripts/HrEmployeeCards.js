function InitGridDoors() {
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
};


function EmployeeCardsViewModel(parentViewModel) {
    var self = {};

    InitGridDoors();

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

    self.ReloadCards = function () {
        self.IsCardClicked(false);

        if (self.EmployeeUID != null) {
            $.getJSON("Employees/GetEmployeeCards/" + self.EmployeeUID, function (cards) {
                ko.mapping.fromJS(cards, {}, self);
            })
            .fail(function (jqxhr, textStatus, error) {
                ShowError(jqxhr.responseText);
            });
        }
    };

    self.InitCards = function (organisationUID, employeeUID) {
        self.OrganisationUID = organisationUID;
        self.EmployeeUID = employeeUID;
        self.ReloadCards();
    };

    self.CanAddCard = ko.computed(function () {
        return app.Menu.HR.IsCardsEditAllowed() && !self.EmployeesParentViewModel.IsOrganisation() && !self.EmployeesParentViewModel.IsDeleted();
    }, self);

    self.CanEditCard = ko.computed(function () {
        return app.Menu.HR.IsCardsEditAllowed();
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
        self.EmployeesParentViewModel.EmployeeCardDetails.InitEmployeeCardDetails(self.OrganisationUID, self.Card.UID(), false);
        ShowBox(box);
    };

    self.AddEmployeeCardClick = function (data, e, box) {
        self.EmployeesParentViewModel.EmployeeCardDetails.InitEmployeeCardDetails(self.OrganisationUID, null, true);
        ShowBox(box);
    };

    self.RemoveEmployeeCardClick = function (data, e) {
        var cardRemovalReason = self.EmployeesParentViewModel.CardRemovalReason;
        cardRemovalReason.Init(function () {
            if (cardRemovalReason.RemoveIsChecked()) {
                var r = confirm("Вы уверены, что хотите удалить карту?");
                if (r) {
                    $.ajax({
                        url: "Employees/DeleteCard",
                        type: "post",
                        contentType: "application/json",
                        data: "{'id':'" + self.Card.UID() + "'}",
                        success: function () {
                            CloseBox();
                            self.ReloadCards();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            ShowError(xhr.responseText);
                        },
                    });
                }
            }
            if (cardRemovalReason.DeactivateIsChecked()) {
                $.ajax({
                    url: "Employees/DeleteFromEmployee",
                    type: "post",
                    contentType: "application/json",
                    data: "{'id': '" + self.Card.UID() + "','employeeName': '" + self.EmployeesParentViewModel.Name() + "','reason': '" + cardRemovalReason.RemovalReason() + "'}",
                    success: function () {
                        CloseBox();
                        self.ReloadCards();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        ShowError(xhr.responseText);
                    },
                });
            }
        });
        ShowBox("#card-removal-reason-box");
    };

    return self;
}