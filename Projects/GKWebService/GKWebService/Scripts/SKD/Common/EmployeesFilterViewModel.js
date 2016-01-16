function InitGridEmployeesFilter() {
    function imageFormat(cellvalue, options, rowObject) {
        var newCellValue = cellvalue;
        if (rowObject.IsDeleted) {
            newCellValue = '<span style="opacity:0.5">' + newCellValue + '</span>';
        };
        if (rowObject.IsOrganisation) {
            newCellValue = '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Organisation.png" />' + newCellValue;
        } else {
            newCellValue = '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Employee.png" />' + newCellValue;
        }
        if (!rowObject.IsOrganisation) {
            newCellValue = '<label><input id="' + rowObject.UID + '" type="checkbox"/>' + newCellValue + '</label>';
        }
        return newCellValue;
    };

    $("#jqGridEmployeesFilter").jqGrid({
        datastr: null,
        datatype: "jsonstring",
        colModel: [
            { label: 'UID', name: 'UID', key: true, hidden: true, sortable: false },
            { label: 'ParentUID', name: 'ParentUID', hidden: true, sortable: false },
            { label: 'OrganisationUID', name: 'OrganisationUID', hidden: true, sortable: false },
            { label: 'IsOrganisation', name: 'IsOrganisation', hidden: true, sortable: false },
            { label: 'ФИО', name: 'Name', width: 100, hidden: false, sortable: false, formatter: imageFormat },
            { label: 'IsDeleted', name: 'IsDeleted', hidden: true, sortable: false }
        ],
        width: 620,
        height: 350,
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

function EmployeesFilterViewModel() {
    var self = {};

    self.IsSearch = ko.observable(false);
    self.LastName = ko.observable();
    self.FirstName = ko.observable();
    self.SecondName = ko.observable();
    self.SelectedPersonType = ko.observable();

    InitGridEmployeesFilter();

    self.Init = function (isWithDeleted, uids, lastName, firstName, secondName, selectedPersonType) {
        self.IsWithDeleted = isWithDeleted;
        self.UIDs = uids;
        self.LastName(lastName);
        self.FirstName(firstName);
        self.SecondName(secondName);
        self.IsSearch(lastName || firstName || secondName);
        self.SelectedPersonType(selectedPersonType);
        self.ReloadTree();
    };

    self.ReloadTree = function () {
        $.getJSON("/Hr/GetEmployeesFilter/",
            { isWithDeleted: self.IsWithDeleted, selectedPersonType: self.SelectedPersonType() },
            function (orgs) {
                self.UpdateTree(orgs);
            })
        .fail(function (jqxhr, textStatus, error) {
            ShowError(jqxhr.responseText);
        });
    };

    self.UpdateTree = function (data) {
        $("#jqGridEmployeesFilter").setGridParam({
            datastr: data,
            datatype: "jsonstring",
            treedatatype: "jsonstring"
        });
        $("#jqGridEmployeesFilter").trigger("reloadGrid");
        $("#jqGridEmployeesFilter").jqGrid("resetSelection");

        ko.utils.arrayForEach(self.UIDs, function(uid) {
            $("#jqGridEmployeesFilter input#" + uid).prop('checked', true);
        });
    };

    self.GetChecked = function () {
        var result = new Array();
        $("#jqGridEmployeesFilter input:checked").each(function(index, value) {
            result.push(value.id);
        });
        return result;
    }

    return self;
}