$(document).ready(function () {
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
});

function DepartmentsViewModel() {
    var self = {};

    self.UID = ko.observable();
    self.ParentUID = ko.observable();
    self.OrganisationUID = ko.observable();
    self.IsOrganisation = ko.observable(true);
    self.ChiefUID = ko.observable();
    self.IsRowSelected = ko.observable(false);
    self.IsDeleted = ko.observable();

    self.CanEdit = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && !self.IsOrganisation();
    }, self);

    self.Init = function (filter) {
        self.Filter = filter;
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
                alert("request failed");
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

    self.EditDepartmentClick = function (data, e, box) {
        self.DepartmentDetails.Init(self.OrganisationUID(), self.UID(), self.ParentUID(), self.ReloadTree );
    };

    return self;
}