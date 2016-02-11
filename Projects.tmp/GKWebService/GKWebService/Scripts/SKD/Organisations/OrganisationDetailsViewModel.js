function OrganisationDetailsViewModel() {
    var self = {};

    self.Title = ko.observable();
    self.IsNew = ko.observable(false);

    self.SelectedChief = ko.observable();
    self.SelectedHRChief = ko.observable();

    self.organisationDetailsPages = {
        General: ko.observable(true),
        Photo: ko.observable(false),
    };

    $.ajax({
        dataType: "json",
        url: "Organisations/GetOrganisationDetails",
        async: false,
        data: null,
        success: function (data) {
            ko.mapping.fromJS(data, {}, self);
        }
    });

    self.Init = function (uid, okClick) {
        $.getJSON("/Organisations/GetOrganisationDetails/",
            { "id": uid },
            function (org) {
                ko.mapping.fromJS(org, {}, self);
                self.OkClick = okClick;
                self.IsNew(false);
                if (uid) {
                    self.Title("Свойства организации: " + self.Organisation.Name());
                } else {
                    self.Title("Создание новой организации");
                    self.IsNew(true);
                }
                ShowBox('#organisation-details-box');
            })
        .fail(function(jqxhr, textStatus, error) {
                ShowError(jqxhr.responseText);
            });
    };

    self.OrganisationDetailsPageClick = function (data, e, page) {
        for (var propertyName in self.organisationDetailsPages) {
            self.organisationDetailsPages[propertyName](false);
        }

        self.organisationDetailsPages[page](!self.organisationDetailsPages[page]());
        $('div#organisation-details-box li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
    };

    self.SelectChief = function () {
        $.getJSON("/Hr/GetOrganisationEmployees/" + self.Organisation.UID(), function (emp) {
            ko.mapping.fromJS(emp, {}, app.Menu.HR.Common.EmployeeSelectionDialog);
            app.Menu.HR.Common.EmployeeSelectionDialog.Init(function (employee) {
                self.SelectedChief(employee);
                if (employee) {
                    self.IsChiefSelected(true);
                } else {
                    self.IsChiefSelected(false);
                }
            });
            ShowBox('#employee-selection-box');
        })
        .fail(function (jqxhr, textStatus, error) {
            ShowError(jqxhr.responseText);
        });
    };

    self.SelectHRChief = function () {
        $.getJSON("/Hr/GetOrganisationEmployees/" + self.Organisation.UID(), function (emp) {
            ko.mapping.fromJS(emp, {}, app.Menu.HR.Common.EmployeeSelectionDialog);
            app.Menu.HR.Common.EmployeeSelectionDialog.Init(function (employee) {
                self.SelectedHRChief(employee);
                if (employee) {
                    self.IsHRChiefSelected(true);
                } else {
                    self.IsHRChiefSelected(false);
                }
            });
            ShowBox('#employee-selection-box');
        })
        .fail(function (jqxhr, textStatus, error) {
            ShowError(jqxhr.responseText);
        });
    };

    self.OrganisationDetailsClose = function () {
        CloseBox();
    };

    self.SaveOrganisation = function () {
        var data = ko.mapping.toJSON(self);
        $.ajax({
            url: "Organisations/OrganisationDetails",
            type: "post",
            contentType: "application/json",
            data: "{'organisation':" + data + ",'isNew': '" + self.IsNew() + "'}",
            success: function (error) {
                if (self.OkClick) {
                    self.OkClick();
                }
                CloseBox();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowError(xhr.responseText);
            }
        });
    };

    return self;
}