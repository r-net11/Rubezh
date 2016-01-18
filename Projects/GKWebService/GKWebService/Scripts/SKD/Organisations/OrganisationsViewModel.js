function OrganisationsViewModel() {
    var self = {};

    self.Organisations = ko.observableArray();
    self.SelectedOrganisation = ko.observable();
    self.Users = ko.observableArray();
    self.Doors = ko.observableArray();

    self.CanAdd = ko.computed(function () {
        return self.SelectedOrganisation() && !self.SelectedOrganisation().IsDeleted() && app.Menu.HR.IsOrganisationsAddRemoveAllowed();
    }, self);
    self.CanRemove = ko.computed(function () {
        return self.SelectedOrganisation() && !self.SelectedOrganisation().IsDeleted() && app.Menu.HR.IsOrganisationsAddRemoveAllowed();
    }, self);
    self.CanEdit = ko.computed(function () {
        return self.SelectedOrganisation() && !self.SelectedOrganisation().IsDeleted() && app.Menu.HR.IsOrganisationsEditAllowed();
    }, self);
    self.CanRestore = ko.computed(function () {
        return self.SelectedOrganisation() && self.SelectedOrganisation().IsDeleted() && app.Menu.HR.IsOrganisationsAddRemoveAllowed();
    }, self);
    self.CanSelectDoors = ko.observable(false);
    self.CanSelectUsers = ko.observable(false);

    self.Init = function (filter) {
        self.Filter = filter;
        self.CanSelectDoors(app.Menu.HR.IsOrganisationsDoorsAllowed());
        self.CanSelectUsers(app.Menu.HR.IsOrganisationsUsersAllowed());
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
                self.SelectedOrganisation(null);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowError(xhr.responseText);
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
                ShowError(xhr.responseText);
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
                ShowError(xhr.responseText);
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
                ShowError(xhr.responseText);
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
                        app.Header.QuestionBox.InitQuestionBox("Существуют карты, шаблоны доступа или графики, привязанные к данной точке доступа. Вы уверены, что хотите снять права с точки доступа?", function() {
                            self.SetDoorChecked(door);
                        });
                    } else {
                        self.SetDoorChecked(door);
                    };
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowError(xhr.responseText);
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
                ShowError(xhr.responseText);
            }
        });
    };

    self.Add = function (data, e) {
        self.OrganisationDetails.Init(null, self.ReloadOrganisationList);
    };

    self.Remove = function (data, e) {
        app.Header.QuestionBox.InitQuestionBox("Вы уверены, что хотите удалить огранизацию?", function () {
            $.getJSON("/Organisations/IsAnyOrganisationItems/", { uid: self.SelectedOrganisation().UID() }, function (isAnyOrganisationItems) {
                if (isAnyOrganisationItems) {
                    app.Header.QuestionBox.InitQuestionBox("Привязанные к организации объекты будут также архивированы. Продолжить?", function () {
                        self.RemoveOrganisation();
                    });
                } else {
                    self.RemoveOrganisation();
                }
            })
            .fail(function (jqxhr, textStatus, error) {
                ShowError(jqxhr.responseText);
            });
        });
    };

    self.RemoveOrganisation = function() {
            $.ajax({
                url: "Organisations/MarkDeleted",
                type: "post",
                contentType: "application/json",
                data: JSON.stringify({ "uid": self.SelectedOrganisation().UID(), "name": self.SelectedOrganisation().Name() }),
                success: function () {
                    self.ReloadOrganisationList();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowError(xhr.responseText);
                }
            });
    };

    self.Edit = function (data, e) {
        self.OrganisationDetails.Init(self.SelectedOrganisation().UID(), self.ReloadOrganisationList );
    };

    self.Restore = function (data, e) {
        app.Header.QuestionBox.InitQuestionBox("Вы уверены, что хотите восстановить огранизацию?", function () {
            $.ajax({
                url: "Organisations/Restore",
                type: "post",
                contentType: "application/json",
                data: JSON.stringify({ "uid": self.SelectedOrganisation().UID(), "name": self.SelectedOrganisation().Name() }),
                success: self.ReloadOrganisationList,
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowError(xhr.responseText);
                }
            });
        });
    };

    return self;
}