function OrganisationsViewModel() {
    var self = {};

    self.Organisations = ko.observableArray();
    self.SelectedOrganisation = ko.observable();
    self.Users = ko.observableArray();
    self.Doors = ko.observableArray();

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

    self.organisationPages = {
        Users: ko.observable(true),
        Doors: ko.observable(false),
    };

    self.OrganisationPageClick = function (data, e, page) {
        for (var propertyName in self.organisationPages) {
            self.organisationPages[propertyName](false);
        }

        self.organisationPages[page](!self.organisationPages[page]());
        $('div#OrganisationsRightPanelTabs li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
    };

    self.ReloadOrganisationList = function () {
        $.ajax({
            url: "/Organisations/GetOrganisations/",
            type: "post",
            contentType: "application/json",
            data: ko.mapping.toJSON(self.Filter),
            success: function (data) {
                ko.mapping.fromJS(data, {}, self);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("request failed");
            },
        });
    };

    self.OrganisationClick = function(data, e, organisation) {
        $('ul#OrganisationList li').removeClass("selected");
        $(e.currentTarget).parent().addClass("selected");
        self.SelectedOrganisation(organisation);
        $.ajax({
            url: "/Organisations/GetOrganisationUsers/",
            type: "post",
            contentType: "application/json",
            data: ko.mapping.toJSON(self.SelectedOrganisation()),
            success: function (users) {
                ko.mapping.fromJS(users, {}, self);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("request failed");
            }
        });
        $.ajax({
            url: "/Organisations/GetOrganisationDoors/",
            type: "post",
            contentType: "application/json",
            data: ko.mapping.toJSON(self.SelectedOrganisation()),
            success: function (doors) {
                ko.mapping.fromJS(doors, {}, self);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("request failed");
            }
        });
    };

    self.UserClick = function (data, e, user) {
        $('ul#OrganisationUsers li').removeClass("selected");
        $(e.currentTarget).parent().addClass("selected");
        $.ajax({
            url: "/Organisations/SetUsersChecked/",
            type: "post",
            contentType: "application/json",
            //dataType: "json",
            async: false,
            data: "{'organisation':" + ko.mapping.toJSON(self.SelectedOrganisation()) + ",'user': " + ko.mapping.toJSON(user) + "}",
            success: function (users) {
                self.SelectedOrganisation().UserUIDs(users.UserUIDs);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("request failed");
            }
        });
    };

    self.DoorClick = function (data, e, door) {
        $('ul#OrganisationDoors li').removeClass("selected");
        $(e.currentTarget).parent().addClass("selected");

        if (!door.IsChecked()) {
            $.ajax({
                url: "/Organisations/IsDoorLinked/",
                type: "post",
                contentType: "application/json",
                async: false,
                data: "{'organisationId': '" + self.SelectedOrganisation().UID() + "','door': " + ko.mapping.toJSON(door) + "}",
                success: function (isDoorLinked) {
                    if (isDoorLinked) {
                        app.Header.QuestionBox.InitQuestionBox("Существуют карты, шаблоны доступа или графики, привязанные к данной точке доступа.<br/>Вы уверены, что хотите снять права с точки доступа?", function() {
                            self.SetDoorChecked(door);
                        });
                    } else {
                        self.SetDoorChecked(door);
                    };
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("request failed");
                }
            });
        } else {
            self.SetDoorChecked(door);
        };

    };

    self.SetDoorChecked = function(door) {
        $.ajax({
            url: "/Organisations/SetDoorsChecked/",
            type: "post",
            contentType: "application/json",
            async: false,
            data: "{'organisation':" + ko.mapping.toJSON(self.SelectedOrganisation()) + ",'door': " + ko.mapping.toJSON(door) + "}",
            success: function (doors) {
                self.SelectedOrganisation().DoorUIDs(doors.DoorUIDs);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("request failed");
            }
        });
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