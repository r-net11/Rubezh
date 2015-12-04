function DepartmentDetailsViewModel(departmentSelectionViewModel) {
    var self = {};

    self.DepartmentSelectionViewModel = departmentSelectionViewModel;
    self.Title = ko.observable();
    self.IsNew = ko.observable(false);
    self.Name = ko.observable();

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
        $.getJSON("/Departments/GetDepartmentDetails/" + uid, function (dep) {
            ko.mapping.fromJS(dep, {}, self);
            self.OkClick = okClick;
            if (uid) {
                self.Title("Свойства подразделения: " + self.Name());
            } else {
                self.Title("Создание подразделения");
                self.IsNew = true;
                self.ParentDepartmentUID(parentUID);
            }
            self.OrganisationUID(organisationUID);
            ShowBox('#department-details-box');
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

    };

    self.SelectDepartment = function () {
        self.DepartmentSelectionViewModel.Init(self.OrganisationUID(), self.UID());
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
            data: "{'department':" + data + ",'isNew': '" + self.IsNew() + "'}",
            success: function (error) {
                if (error) {
                    alert(error);
                };
                if (self.OkClick) {
                    self.OkClick();
                }
                CloseBox();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("request failed");
            }
        });
    };

    return self;
}