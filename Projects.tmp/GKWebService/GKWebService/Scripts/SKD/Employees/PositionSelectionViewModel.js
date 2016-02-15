var InitGridPositionSelection = function () {
    $("#jqGridPositionSelection").jqGrid({
        datastr: null,
        datatype: "jsonstring",
        colModel: [
            { label: 'UID', name: 'UID', key: true, hidden: true, sortable: false },
            { label: 'Название', name: 'Name', width: 100, hidden: false, sortable: false },
            { label: 'Описание', name: 'Description', width: 100, hidden: false, sortable: false }
        ],
        width: 630,
        height: 500,
        rowNum: 100,
        viewrecords: true
    });
};

function PositionSelectionViewModel() {
    var self = {};

    InitGridPositionSelection();

    self.Title = ko.observable();
    self.UID = ko.observable();
    self.Name = ko.observable();

    self.Init = function (organisationUID, positionUID, save) {
        self.OrganisationUID = organisationUID;
        self.PositionUID = positionUID;
        self.Save = save;
        self.UID(null);
        self.ReloadTree();
        self.Title("Выбор должности");
        ShowBox('#position-selection-box');
    };

    self.ReloadTree = function () {
        $.ajax({
            url: "/Employees/GetPositions",
            type: "post",
            contentType: "application/json",
            data: "{'organisationUID': '" + self.OrganisationUID + "', 'positionUID': '" + self.PositionUID + "'}",
            success: function (data) {
                self.UpdateTree(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowError(xhr.responseText);
            }
        });

    };

    self.UpdateTree = function (data) {
        $("#jqGridPositionSelection").setGridParam({
            datastr: data,
            datatype: "jsonstring",
        });
        $("#jqGridPositionSelection").trigger("reloadGrid");
        $("#jqGridPositionSelection").jqGrid("resetSelection");
    };

    $('#jqGridPositionSelection').on('jqGridSelectRow', function (event, id, selected) {

        if (selected) {
            self.UID(id);
            var rowData = $("#jqGridPositionSelection").getRowData(id);
            self.Name(rowData.Name);
        }
    });

    self.OkClick = function () {
        self.Save(self.UID(), self.Name());
        $('#position-selection-box').fadeOut(300);
    };

    self.Clear = function () {
        self.Save(null, null);
        $('#position-selection-box').fadeOut(300);
    };

    self.Add = function () {
        app.Menu.HR.Departments.PositionDetails.Init(self.OrganisationUID, null, self.ReloadTree);
    };


    self.Close = function () {
        $('#position-selection-box').fadeOut(300);
    };

    return self;
}