var InitGridDepartmentSelection = function() {
    $("#jqGridDepartmentSelection").jqGrid({
        datastr: null,
        datatype: "jsonstring",
        colModel: [
            { label: 'UID', name: 'UID', key: true, hidden: true, sortable: false },
            { label: 'ParentUID', name: 'ParentUID', hidden: true, sortable: false },
            { label: 'Название', name: 'Name', width: 100, hidden: false, sortable: false }
        ],
        width: 630,
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

function DepartmentSelectionViewModel() {
    var self = {};

    InitGridDepartmentSelection();

    self.Title = ko.observable();
    self.UID = ko.observable();
    self.Name = ko.observable();


    self.Init = function (organisationUID, departmentUID, save) {
        self.OrganisationUID = organisationUID;
        self.DepartmentUID = departmentUID;
        self.Save = save;
        self.UID(null);
        self.ReloadTree();
        if (departmentUID) {
            self.Title("Выбор родительского подразделения");
        } else {
            self.Title("Выбор подразделения");
        }
        ShowBox('#department-selection-box');
    };

    self.ReloadTree = function () {
        $.ajax({
            url: "/Employees/GetDepartments",
            type: "post",
            contentType: "application/json",
            data: "{'organisationUID': '" + self.OrganisationUID + "', 'departmentUID': '" + self.DepartmentUID + "'}",
            success: function (data) {
                self.UpdateTree(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowError(xhr.responseText);
            }
        });

    };

    self.UpdateTree = function (data) {
        $("#jqGridDepartmentSelection").setGridParam({
            datastr: data,
            datatype: "jsonstring",
            treedatatype: "jsonstring"
        });
        $("#jqGridDepartmentSelection").trigger("reloadGrid");
        $("#jqGridDepartmentSelection").jqGrid("resetSelection");
    };

    $('#jqGridDepartmentSelection').on('jqGridSelectRow', function (event, id, selected) {

        if (selected) {
            self.UID(id);
            var rowData = $("#jqGridDepartmentSelection").getRowData(id);
            self.Name(rowData.Name);
        }
    });

    self.OkClick = function () {
        self.Save(self.UID(), self.Name());
        $('#department-selection-box').fadeOut(300);
    };

    self.Clear = function () {
        self.Save(null, null);
        $('#department-selection-box').fadeOut(300);
    };

    self.Add = function () {
        var parentDepartment = (self.UID ? self.UID : null);
        app.Menu.HR.Departments.DepartmentDetails.Init(self.OrganisationUID, null, parentDepartment, self.ReloadTree);
    };


    self.Close = function () {
        $('#department-selection-box').fadeOut(300);
    };

    return self;
}