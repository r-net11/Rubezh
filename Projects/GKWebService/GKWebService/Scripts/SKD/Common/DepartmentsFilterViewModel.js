function InitGridDepartmentsFilter() {
    function imageFormat(cellvalue, options, rowObject) {
        var newCellValue = cellvalue;
        if (rowObject.IsDeleted) {
            newCellValue = '<span style="opacity:0.5">' + newCellValue + '</span>';
        };
        if (rowObject.IsOrganisation) {
            newCellValue = '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Organisation.png" />' + newCellValue;
        } else {
            newCellValue = '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Department.png" />' + newCellValue;
        }
        if (!rowObject.IsOrganisation) {
            newCellValue = '<label><input id="' + rowObject.UID + '" type="checkbox"/>' + newCellValue + '</label>';
        }
        return newCellValue;
    };

    $("#jqGridDepartmentsFilter").jqGrid({
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
            { label: 'IsDeleted', name: 'IsDeleted', hidden: true, sortable: false }
        ],
        width: 620,
        height: 500,
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

function DepartmentsFilterViewModel() {
    var self = {};

    InitGridDepartmentsFilter();

    self.Init = function (isWithDeleted, uids) {
        self.IsWithDeleted = isWithDeleted;
        self.UIDs = uids;
        self.ReloadTree();
    };

    self.ReloadTree = function () {
        $.getJSON("/Hr/GetDepartmentsFilter/",
            { isWithDeleted: self.IsWithDeleted },
            function (orgs) {
                self.UpdateTree(orgs);
            })
            .fail(function (jqxhr, textStatus, error) {
                ShowError(jqxhr.responseText);
            });

    };

    self.UpdateTree = function (data) {
        $("#jqGridDepartmentsFilter").setGridParam({
            datastr: data,
            datatype: "jsonstring",
            treedatatype: "jsonstring"
        });
        $("#jqGridDepartmentsFilter").trigger("reloadGrid");
        $("#jqGridDepartmentsFilter").jqGrid("resetSelection");

        ko.utils.arrayForEach(self.UIDs, function(uid) {
            $("#jqGridDepartmentsFilter input#" + uid).prop('checked', true);
        });
    };

    self.GetChecked = function () {
        var result = new Array();
        $("#jqGridDepartmentsFilter input:checked").each(function(index, value) {
            result.push(value.id);
        });
        return result;
    }


    return self;
}