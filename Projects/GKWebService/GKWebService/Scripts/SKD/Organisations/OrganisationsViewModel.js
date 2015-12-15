function OrganisationsViewModel() {
    var self = {};

    self.Organisations = ko.observableArray();
    self.SelectedOrganisation = ko.observable();

    self.CanAdd = ko.computed(function () {
        return self.SelectedOrganisation() && !self.SelectedOrganisation().IsDeleted();
    }, self);
    self.CanRemove = ko.computed(function () {
        return self.SelectedOrganisation() && !self.SelectedOrganisation().IsDeleted();
    }, self);
    self.CanEdit = ko.computed(function () {
        return self.SelectedOrganisation() && !self.SelectedOrganisation().IsDeleted();
    }, self);
    self.CanRestore = ko.computed(function () {
        return self.SelectedOrganisation() && self.SelectedOrganisation().IsDeleted();
    }, self);

    self.Init = function (filter) {
        self.Filter = filter;
        self.ReloadOrganisationList();
    };

    self.ReloadOrganisationList = function() {
        $.getJSON("/Organisations/GetOrganisations/",
            ko.mapping.toJSON(self.Filter),
            function (organisations) {
                ko.mapping.fromJS(organisations, {}, self);
            });
    };

    self.OrganisationClick = function(data, e, organisation) {
        $('ul#OrganisationList li').removeClass("selected");
        $(e.currentTarget).parent().addClass("selected");
        self.SelectedOrganisation(organisation);
    };

    self.Add = function (data, e) {
        self.PositionDetails.Init(self.OrganisationUID(), '', self.ReloadTree);
    };

    self.Remove = function (data, e) {
        app.Header.QuestionBox.InitQuestionBox("Вы уверены, что хотите архивировать должность?", function () {
            $.getJSON("/Positions/GetChildEmployeeUIDs/", { positionId: self.UID() }, function (employeeUIDs) {
                if (employeeUIDs.length > 0) {
                    app.Header.QuestionBox.InitQuestionBox("Существуют привязанные к должности сотрудники. Продолжить?", function() {
                        self.RemovePosition();
                    });
                } else {
                    self.RemovePosition();
                }
            });
        });
    };

    self.RemovePosition = function() {
            $.ajax({
                url: "Positions/MarkDeleted",
                type: "post",
                contentType: "application/json",
                data: JSON.stringify({ "uid": self.UID(), "name": self.Name() }),
                success: function () {
                    self.ReloadOrganisationList();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("request failed");
                }
            });
    };

    self.Edit = function (data, e) {
        self.OrganisationDetails.Init(self.SelectedOrganisation().UID(), self.ReloadOrganisationList );
    };

    self.Restore = function (data, e) {
        app.Header.QuestionBox.InitQuestionBox("Вы уверены, что хотите восстановить должность?", function () {
            var ids = $("#jqGridPositions").getDataIDs();
            for (var i = 0; i < ids.length; i++) {
                var rowData = $("#jqGridPositions").getRowData(ids[i]);
                if (rowData.IsDeleted !== "true" &&
                    rowData.Name === self.Name() &&
                    rowData.OrganisationUID === self.OrganisationUID() &&
                    !rowData.IsOrganisation) {
                    alert("Существует неудалённый элемент с таким именем");
                    return;
                }
            }

            $.ajax({
                url: "Positions/Restore",
                type: "post",
                contentType: "application/json",
                data: JSON.stringify({ "uid": self.UID(), "name": self.Name() }),
                success: function () {
                    self.ReloadTree();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("request failed");
                }
            });
        });
    };

    return self;
}