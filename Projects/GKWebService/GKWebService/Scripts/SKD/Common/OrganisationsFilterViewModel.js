function InitGridOrganisationsFilter() {
    function imageFormat(cellvalue, options, rowObject) {
        var newCellValue = cellvalue;
        if (rowObject.IsDeleted) {
            newCellValue = '<span style="opacity:0.5">' + newCellValue + '</span>';
        };
        return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Organisation.png" />' + newCellValue;
        newCellValue = '<label><input type="checkbox" />' + newCellValue + '</label>';
        return newCellValue;
    };

    $("#jqGridOrganisationsFilter").jqGrid({
        datastr: null,
        datatype: "jsonstring",
        colModel: [
            { label: ' ', name: 'IsChecked', editable: true, edittype: 'checkbox', editoptions: { value: "True:False" }, formatter: "checkbox", formatoptions: { disabled: false }, sortable: false, width: 7 },
            { label: 'UID', name: 'UID', key: true, hidden: true, sortable: false },
            { label: 'Название', name: 'Name', width: 100, hidden: false, sortable: false, formatter: imageFormat },
            { label: 'Примечание', name: 'Description', width: 100, hidden: false, sortable: false},
            { label: 'IsDeleted', name: 'IsDeleted', hidden: true, sortable: false }
        ],
        width: 620,
        height: 500,
        rowNum: 100,
        viewrecords: true
    });
};

function OrganisationsFilterViewModel() {
    var self = {};

    InitGridOrganisationsFilter();

    self.Init = function (isWithDeleted, uids) {
        self.IsWithDeleted = isWithDeleted;
        self.UIDs = uids;
        self.ReloadTree();
    };

    self.ReloadTree = function () {
        $.getJSON("/Hr/GetOrganisationsFilter/",
            { isWithDeleted: self.IsWithDeleted },
            function (orgs) {
                self.UpdateTree(orgs);
            })
        .fail(function (jqxhr, textStatus, error) {
            ShowError(jqxhr.responseText);
        });
    };

    self.UpdateTree = function (data) {
        $("#jqGridOrganisationsFilter").setGridParam({
            datastr: data,
            datatype: "jsonstring",
            treedatatype: "jsonstring",
        });
        $("#jqGridOrganisationsFilter").trigger("reloadGrid");
        $("#jqGridOrganisationsFilter").jqGrid("resetSelection");
        var ids = $("#jqGridOrganisationsFilter").getDataIDs();
        for (var i = 0; i < ids.length; i++) {
            $("#jqGridOrganisationsFilter").setCell(ids[i], "IsChecked", self.UIDs.indexOf(ids[i]) === -1 ? "False" : "True");
        }
    };

    self.GetChecked = function() {
        var result = new Array();
        var ids = $("#jqGridOrganisationsFilter").getDataIDs();
        for (var i = 0; i < ids.length; i++) {
            var rowData = $("#jqGridOrganisationsFilter").getRowData(ids[i]);
            if (rowData.IsChecked === "True") {
                result.push(rowData.UID);
            }
        }
        return result;
    }


    return self;
}