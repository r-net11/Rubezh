function DepartmentDetailsViewModel(departmentSelectionViewModel) {
    var self = {};

    self.DepartmentSelectionViewModel = departmentSelectionViewModel;
    self.Title = ko.observable();
    self.IsNew = ko.observable(false);

    self.departmentDetailsPages = {
        General: ko.observable(true),
        Photo: ko.observable(false),
    };

    $.ajax({
        dataType: "json",
        url: "Departments/GetDepartmentDetails",
        async: false,
        data: null,
        success: function (data) {
            ko.mapping.fromJS(data, {}, self);
        }
    });

    self.Init = function (organisationUID, uid, parentUID, okClick) {
        $.getJSON("/Departments/GetDepartmentDetails/",
            { organisationId: organisationUID, id: uid, parentDepartmentId: parentUID },
            function (dep) {
                ko.mapping.fromJS(dep, {}, self);
                self.OkClick = okClick;
                self.IsNew(false);
                if (uid) {
                    self.Title("Свойства подразделения: " + self.Department.Name());
                } else {
                    self.Title("Создание подразделения");
                    self.IsNew(true);
                }
                ShowBox('#department-details-box');
        })
        .fail(function (jqxhr, textStatus, error) {
            ShowError(jqxhr.responseText);
        });
    };

    self.DepartmentDetailsPageClick = function (data, e, page) {
        for (var propertyName in self.departmentDetailsPages) {
            self.departmentDetailsPages[propertyName](false);
        }

        self.departmentDetailsPages[page](!self.departmentDetailsPages[page]());
        $('div#department-details-box li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
    };

    self.SelectChief = function () {
        $.getJSON("/Hr/GetDepartmentEmployees/" + self.Department.UID(), function (emp) {
            ko.mapping.fromJS(emp, {}, app.Menu.HR.Common.EmployeeSelectionDialog);
            app.Menu.HR.Common.EmployeeSelectionDialog.Init(function(employee) {
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

    self.SelectDepartment = function () {
        self.DepartmentSelectionViewModel.Init(self.Department.OrganisationUID(), self.Department.UID(), function (uid, name) {
            if (uid) {
                self.IsDepartmentSelected(true);
                self.SelectedDepartment().UID(uid);
                self.SelectedDepartment().Name(name);
            } else {
                self.IsDepartmentSelected(false);
            }
        });
    };

    self.DepartmentDetailsClose = function () {
        CloseBox();
    };

    self.SaveDepartment = function () {
        var data = ko.mapping.toJSON(self);
        $.ajax({
            url: "Departments/DepartmentDetails",
            type: "post",
            contentType: "application/json",
            data: "{'departmentModel':" + data + ",'isNew': '" + self.IsNew() + "'}",
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