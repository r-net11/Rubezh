function InitGridDepartments() {
    function imageFormat(cellvalue, options, rowObject) {
        var newCellValue = cellvalue;
        if (rowObject.IsDeleted) {
            newCellValue = '<span style="opacity:0.5">' + newCellValue + '</span>';
        };
        if (rowObject.IsOrganisation) {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Organisation.png" />' + newCellValue;
        } else {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Department.png" />' + newCellValue;
        }
    };

    $("#jqGridDepartments").jqGrid({
        datastr: null,
        datatype: "jsonstring",
        colModel: [
            { label: 'UID', name: 'UID', key: true, hidden: true, sortable: false },
            { label: 'ParentUID', name: 'ParentUID', hidden: true, sortable: false },
            { label: 'OrganisationUID', name: 'OrganisationUID', hidden: true, sortable: false },
            { label: 'IsOrganisation', name: 'IsOrganisation', hidden: true, sortable: false },
            { label: 'Название', name: 'Name', width: 100, hidden: false, sortable: false, formatter: imageFormat },
            { label: 'Name', name: 'NameData', width: 100, hidden: true, sortable: false },
            { label: 'Примечание', name: 'Description', hidden: false, sortable: false },
            { label: 'Телефон', name: 'Model.Phone', hidden: false, sortable: false },
            { label: 'ChiefUID', name: 'Model.ChiefUID', hidden: true, sortable: false },
            { label: 'IsDeleted', name: 'IsDeleted', hidden: true, sortable: false }
        ],
        width: jQuery(window).width() - 242,
        height: 250,
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

function DepartmentsViewModel() {
    var self = {};

    InitGridDepartments();

    self.UID = ko.observable();
    self.ParentUID = ko.observable();
    self.OrganisationUID = ko.observable();
    self.Name = ko.observable();
    self.NameData = ko.observable();
    self.IsOrganisation = ko.observable(true);
    self.ChiefUID = ko.observable();
    self.IsRowSelected = ko.observable(false);
    self.IsDeleted = ko.observable();
    self.Clipboard = ko.observable();

    self.CanAdd = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && app.Menu.HR.IsDepartmentsEditAllowed();
    }, self);
    self.CanRemove = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && !self.IsOrganisation() && app.Menu.HR.IsDepartmentsEditAllowed();
    }, self);
    self.CanEdit = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && !self.IsOrganisation() && app.Menu.HR.IsDepartmentsEditAllowed();
    }, self);
    self.CanCopy = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && !self.IsOrganisation() && app.Menu.HR.IsDepartmentsEditAllowed();
    }, self);
    self.CanPaste = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && self.Clipboard() && app.Menu.HR.IsDepartmentsEditAllowed();
    }, self);
    self.CanRestore = ko.computed(function () {
        return self.IsRowSelected() && self.IsDeleted() && !self.IsOrganisation() && app.Menu.HR.IsDepartmentsEditAllowed();
    }, self);
    self.IsShowEmployeeList = ko.computed(function () {
        return !self.IsOrganisation() && app.Menu.HR.IsEmployeesViewAllowed();
    }, self);


    self.Init = function (filter) {
        self.Filter = filter;
        self.Clipboard(null);
        self.ReloadTree();
    };

    self.ReloadTree = function () {
        self.IsOrganisation(true);
        self.IsRowSelected(false);

        $.ajax({
            url: "/Departments/GetOrganisations",
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
        $("#jqGridDepartments").setGridParam({
            datastr: data,
            datatype: "jsonstring",
            treedatatype: "jsonstring",
        });
        $("#jqGridDepartments").trigger("reloadGrid");
        $("#jqGridDepartments").jqGrid("resetSelection");
    };

    $('#jqGridDepartments').on('jqGridSelectRow', function (event, id, selected) {

        if (selected) {
            var myGrid = $('#jqGridDepartments');

            self.UID(id);
            self.OrganisationUID(myGrid.jqGrid('getCell', id, 'OrganisationUID'));
            self.ParentUID(myGrid.jqGrid('getCell', id, 'ParentUID'));
            self.Name(myGrid.jqGrid('getCell', id, 'Name'));
            self.NameData(myGrid.jqGrid('getCell', id, 'NameData'));
            self.IsOrganisation(myGrid.jqGrid('getCell', id, 'IsOrganisation') == "true");
            self.ChiefUID(myGrid.jqGrid('getCell', id, 'Model.ChiefUID'));
            self.IsDeleted(myGrid.jqGrid('getCell', id, 'IsDeleted') == "true");

            self.DepartmentEmployeeList.Init();
/*
            if (!self.IsOrganisation()) {
                self.EmployeeCards.InitCards(self.OrganisationUID(), self.UID());
            } else {
                self.EmployeeCards.InitCards(self.OrganisationUID(), null);
            }
*/

            self.IsRowSelected(true);
        }
    });

    self.Add = function (data, e) {
        self.DepartmentDetails.Init(self.OrganisationUID(), '', self.UID(), self.ReloadTree);
    };

    self.Remove = function (data, e) {
        app.Header.QuestionBox.InitQuestionBox("Вы уверены, что хотите архивировать подразделение?", function () {
            $.getJSON("/Departments/GetChildEmployeeUIDs/", { departmentId: self.UID() }, function (departmentUIDs) {
                if (departmentUIDs.length > 0) {
                    app.Header.QuestionBox.InitQuestionBox("Существуют привязанные к подразделению сотрудники. Продолжить?", function() {
                        self.RemoveDepartment();
                    });
                } else {
                    self.RemoveDepartment();
                }
            })
            .fail(function (jqxhr, textStatus, error) {
                ShowError(jqxhr.responseText);
            });
        });
    };

    self.RemoveDepartment = function() {
            $.ajax({
                url: "Departments/MarkDeleted",
                type: "post",
                contentType: "application/json",
                data: JSON.stringify({ "uid": self.UID() }),
                success: function () {
                    self.ReloadTree();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowError(xhr.responseText);
                }
            });
    };

    self.EditDepartmentClick = function (data, e, box) {
        self.DepartmentDetails.Init(self.OrganisationUID(), self.UID(), self.ParentUID(), self.ReloadTree );
    };

    self.Copy = function (data, e) {
    };

    self.Paste = function (data, e) {
    };

    self.Restore = function (data, e) {
        app.Header.QuestionBox.InitQuestionBox("Вы уверены, что хотите восстановить подразделение?", function () {
            var ids = $("#jqGridDepartments").getDataIDs();
            for (var i = 0; i < ids.length; i++) {
                var rowData = $("#jqGridDepartments").getRowData(ids[i]);
                if (rowData.IsDeleted !== "true" &&
                    rowData.Name === self.Name() &&
                    rowData.OrganisationUID === self.OrganisationUID() &&
                    !rowData.IsOrganisation) {
                    alert("Существует неудалённый элемент с таким именем");
                    return;
                }
            }

            $.ajax({
                url: "Departments/Restore",
                type: "post",
                contentType: "application/json",
                data: JSON.stringify({ "uid": self.UID() }),
                success: function () {
                    self.ReloadTree();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowError(xhr.responseText);
                }
            });
        });
    };

    self.SetChief = function (chiefUID) {
        $("#jqGridDepartments").setCell(self.UID(), "Model.ChiefUID", chiefUID);
        self.ChiefUID(chiefUID);
    };

    return self;
}