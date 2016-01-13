function InitGridEmployees() {
    function imageFormat(cellvalue, options, rowObject) {
        var newCellValue = cellvalue;
        if (rowObject.IsDeleted) {
            newCellValue = '<span style="opacity:0.5">' + newCellValue + '</span>';
        };
        if (rowObject.IsOrganisation) {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Organisation.png" />' + newCellValue;
        } else {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Employee.png" />' + newCellValue;
        }
    };

    $("#jqGridEmployees").jqGrid({
        datastr: null,
        datatype: "jsonstring",
        colModel: [
            { label: 'UID', name: 'UID', key: true, hidden: true, sortable: false },
            { label: 'ParentUID', name: 'ParentUID', hidden: true, sortable: false },
            { label: 'OrganisationUID', name: 'OrganisationUID', hidden: true, sortable: false },
            { label: 'ФИО', name: 'Name', width: 100, sortable: false, formatter: imageFormat },
            { label: 'Name', name: 'NameData', width: 100, hidden: true, sortable: false},
            { label: 'Подразделение', name: 'Model.DepartmentName', width: 100, sortable: false },
            { label: 'IsOrganisation', name: 'IsOrganisation', hidden: true, sortable: false },
            { label: 'LastName', name: 'Model.LastName', hidden: true, sortable: false },
            { label: 'FirstName', name: 'Model.FirstName', hidden: true, sortable: false },
            { label: 'SecondName', name: 'Model.SecondName', hidden: true, sortable: false },
            { label: 'Phone', name: 'Model.Phone', hidden: true, sortable: false },
            { label: 'Description', name: 'Description', hidden: true, sortable: false },
            { label: 'PositionName', name: 'Model.PositionName', hidden: true, sortable: false },
            { label: 'DepartmentName', name: 'Model.DepartmentName', hidden: true, sortable: false },
            { label: 'OrganisationName', name: 'Model.OrganisationName', hidden: true, sortable: false },
            { label: 'RemovalDate', name: 'RemovalDate', hidden: true, sortable: false },
            { label: 'IsDeleted', name: 'IsDeleted', hidden: true, sortable: false }
        ],
        width: jQuery(window).width() - 242,
        height: 300,
        rowNum: 100,
        viewrecords: true,

        treeGrid: true,
        ExpandColumn: "Name",
        treedatatype: "local",
        treeGridModel: "adjacency",
        loadonce: false,
        treeReader: {
            parent_id_field: "ParentUID",
            level_field: "Level",
            leaf_field: "IsLeaf",
            expanded_field: "IsExpanded"
        }
    });
};

function EmployeesViewModel(parentViewModel) {
    var self = {};

    self.ParentViewModel = parentViewModel;

    InitGridEmployees();

    self.UID = ko.observable();
    self.OrganisationUID = ko.observable();
    self.DepartmentName = ko.observable();
    self.IsOrganisation = ko.observable(true);
    self.IsGuest = ko.observable();
    self.LastName = ko.observable();
    self.FirstName = ko.observable();
    self.SecondName = ko.observable();
    self.Name = ko.observable();
    self.Phone = ko.observable();
    self.Description = ko.observable();
    self.PositionName = ko.observable();
    self.OrganisationName = ko.observable();
    self.RemovalDate = ko.observable();
    self.IsDeleted = ko.observable();
    self.IsRowSelected = ko.observable(false);
    self.PhotoData = ko.observable();
    self.ItemRemovingName = ko.computed(function() {
        return self.IsGuest() ? "посетителя" : "сотрудника";
    }, self);
    self.AddCommandToolTip = ko.computed(function() {
        return "Добавить " + self.ItemRemovingName();
    }, self);
    self.RemoveCommandToolTip = ko.computed(function () {
        return "Удалить " + self.ItemRemovingName();
    }, self);
    self.EditCommandToolTip = ko.computed(function () {
        return "Редактировать " + self.ItemRemovingName();
    }, self);
    self.CanAdd = ko.computed(function() {
        return self.IsRowSelected() && !self.IsDeleted() && self.IsEditAllowed();
    }, self);
    self.CanRemove = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && !self.IsOrganisation() && self.IsEditAllowed();
    }, self);
    self.CanEdit = ko.computed(function() {
        return self.IsRowSelected() && !self.IsDeleted() && !self.IsOrganisation() && self.IsEditAllowed();
    }, self);
    self.CanRestore = ko.computed(function () {
        return self.IsRowSelected() && self.IsDeleted() && !self.IsOrganisation() && self.IsEditAllowed();
    }, self);
    self.IsEditAllowed = ko.computed(function () {
        return self.IsGuest() ? app.Menu.HR.IsGuestEditAllowed : app.Menu.HR.IsEmployeesEditAllowed;
    }, self);

    self.Init = function(filter) {
        self.Filter = filter;
        self.ReloadTree();
    };

    self.ReloadTree = function () {
        self.IsGuest(parentViewModel.SelectedPersonType() === "Guest");
        self.IsOrganisation(true);
        self.IsRowSelected(false);

        $.ajax({
            url: "/Employees/GetOrganisations",
            type: "post",
            contentType: "application/json",
            data: ko.mapping.toJSON(self.Filter),
            success: function (data) {
                self.UpdateTree(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowError(xhr.responseText);
            },
        });

    };

    self.UpdateTree = function (data) {
        $("#jqGridEmployees").setGridParam({
            datastr: data,
            datatype: "jsonstring",
            treedatatype: "jsonstring",
        });
        $("#jqGridEmployees").trigger("reloadGrid");
        $("#jqGridEmployees").jqGrid("resetSelection");
    };

    $('#jqGridEmployees').on('jqGridSelectRow', function (event, id, selected) {

        if (selected) {
            var myGrid = $('#jqGridEmployees');

            self.UID(id);
            self.OrganisationUID(myGrid.jqGrid('getCell', id, 'OrganisationUID'));
            self.DepartmentName(myGrid.jqGrid('getCell', id, 'Model.DepartmentName'));
            self.IsOrganisation(myGrid.jqGrid('getCell', id, 'IsOrganisation') == "true");
            self.LastName(myGrid.jqGrid('getCell', id, 'Model.LastName'));
            self.FirstName(myGrid.jqGrid('getCell', id, 'Model.FirstName'));
            self.SecondName(myGrid.jqGrid('getCell', id, 'Model.SecondName'));
            self.Name(myGrid.jqGrid('getCell', id, 'NameData'));
            self.Phone(myGrid.jqGrid('getCell', id, 'Model.Phone'));
            self.Description(myGrid.jqGrid('getCell', id, 'Description'));
            self.PositionName(myGrid.jqGrid('getCell', id, 'Model.PositionName'));
            self.OrganisationName(myGrid.jqGrid('getCell', id, 'Model.OrganisationName'));
            self.RemovalDate(myGrid.jqGrid('getCell', id, 'RemovalDate'));
            self.IsDeleted(myGrid.jqGrid('getCell', id, 'IsDeleted') == "true");

            if (!self.IsOrganisation()) {
                $.getJSON("/Employees/GetEmployeePhoto/" + self.UID(), function (photo) {
                    self.PhotoData(photo);
                })
                .fail(function (jqxhr, textStatus, error) {
                    ShowError(jqxhr.responseText);
                });
            }

            if (!self.IsOrganisation()) {
                self.EmployeeCards.InitCards(self.OrganisationUID(), self.UID());
            } else {
                self.EmployeeCards.InitCards(self.OrganisationUID(), null);
            }

            self.IsRowSelected(true);
        }
    });

    self.AddEmployeeClick = function(data, e, box) {
        $.getJSON("/Employees/GetEmployeeDetails/", function(emp) {
            ko.mapping.fromJS(emp, {}, self.EmployeeDetails);
            $.getJSON("/Employees/GetOrganisation/" + self.OrganisationUID(), function(org) {
                self.EmployeeDetails.Organisation = org;
                self.EmployeeDetails.Init(true, self.ParentViewModel.SelectedPersonType(), self.ReloadTree);
                ShowBox(box);
            })
            .fail(function (jqxhr, textStatus, error) {
                ShowError(jqxhr.responseText);
            });
        })
        .fail(function (jqxhr, textStatus, error) {
            ShowError(jqxhr.responseText);
        });
    };

    self.RemoveEmployeeClick = function (data, e) {
        app.Header.QuestionBox.InitQuestionBox("Вы уверены, что хотите архивировать " + self.ItemRemovingName() + "?", function () {
            var jsonData = "{'uid':'" + self.UID() + "','name': '" + self.Name() + "','isOrganisation': '" + self.IsOrganisation() + "'}";
            $.ajax({
                url: "Employees/MarkDeleted",
                type: "post",
                contentType: "application/json",
                data: jsonData,
                success: function () {
                    self.ReloadTree();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("request failed");
                },
            });
        });
    };

    self.EditEmployeeClick = function(data, e, box) {
        $.getJSON("/Employees/GetEmployeeDetails/" + self.UID(), function(emp) {
            ko.mapping.fromJS(emp, {}, self.EmployeeDetails);
            $.getJSON("/Employees/GetOrganisation/" + self.OrganisationUID(), function(org) {
                self.EmployeeDetails.Organisation = org;
                self.EmployeeDetails.Init(false, self.ParentViewModel.SelectedPersonType(), self.ReloadTree);
                ShowBox(box);
            })
            .fail(function (jqxhr, textStatus, error) {
                ShowError(jqxhr.responseText);
            });
        })
        .fail(function (jqxhr, textStatus, error) {
            ShowError(jqxhr.responseText);
        });
    };

    self.RestoreEmployeeClick = function (data, e) {
        app.Header.QuestionBox.InitQuestionBox("Вы уверены, что хотите восстановить " + self.ItemRemovingName() + "?", function () {
            var ids = $("#jqGridEmployees").getDataIDs();
            for (var i=0; i < ids.length; i++){
                var rowData = $("#jqGridEmployees").getRowData(ids[i]);
                if (rowData.IsDeleted !== "true" &&
                    rowData.Name === self.Name() &&
                    rowData.OrganisationUID === self.OrganisationUID() &&
                    !rowData.IsOrganisation) {
                    alert("Существует неудалённый элемент с таким именем");
                    return;
                }
            }
            var jsonData = "{'uid':'" + self.UID() + "','name': '" + self.Name() + "','isOrganisation': '" + self.IsOrganisation() + "'}";
            $.ajax({
                url: "Employees/Restore",
                type: "post",
                contentType: "application/json",
                data: jsonData,
                success: function () {
                    self.ReloadTree();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("request failed");
                },
            });
        });
    };

    return self;
}
