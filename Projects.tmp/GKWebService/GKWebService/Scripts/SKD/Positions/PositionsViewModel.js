function InitGridPositions() {
    function imageFormat(cellvalue, options, rowObject) {
        var newCellValue = cellvalue;
        if (rowObject.IsDeleted) {
            newCellValue = '<span style="opacity:0.5">' + newCellValue + '</span>';
        };
        if (rowObject.IsOrganisation) {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Organisation.png" />' + newCellValue;
        } else {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Position.png" />' + newCellValue;
        }
    };

    $("#jqGridPositions").jqGrid({
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

function PositionsViewModel() {
    var self = {};

    InitGridPositions();

    self.UID = ko.observable();
    self.ParentUID = ko.observable();
    self.OrganisationUID = ko.observable();
    self.Name = ko.observable();
    self.NameData = ko.observable();
    self.Description = ko.observable();
    self.IsOrganisation = ko.observable(true);
    self.IsRowSelected = ko.observable(false);
    self.IsDeleted = ko.observable();
    self.Clipboard = ko.observable();

    self.CanAdd = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && app.Menu.HR.IsPositionsEditAllowed();
    }, self);
    self.CanRemove = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && !self.IsOrganisation() && app.Menu.HR.IsPositionsEditAllowed();
    }, self);
    self.CanEdit = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && !self.IsOrganisation() && app.Menu.HR.IsPositionsEditAllowed();
    }, self);
    self.CanCopy = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && !self.IsOrganisation() && app.Menu.HR.IsPositionsEditAllowed();
    }, self);
    self.CanPaste = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && self.Clipboard() && app.Menu.HR.IsPositionsEditAllowed();
    }, self);
    self.CanRestore = ko.computed(function () {
        return self.IsRowSelected() && self.IsDeleted() && !self.IsOrganisation() && app.Menu.HR.IsPositionsEditAllowed();
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
            url: "/Positions/GetOrganisations",
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
        $("#jqGridPositions").setGridParam({
            datastr: data,
            datatype: "jsonstring",
            treedatatype: "jsonstring",
        });
        $("#jqGridPositions").trigger("reloadGrid");
        $("#jqGridPositions").jqGrid("resetSelection");
    };

    $('#jqGridPositions').on('jqGridSelectRow', function (event, id, selected) {

        if (selected) {
            var myGrid = $('#jqGridPositions');

            self.UID(id);
            self.OrganisationUID(myGrid.jqGrid('getCell', id, 'OrganisationUID'));
            self.ParentUID(myGrid.jqGrid('getCell', id, 'ParentUID'));
            self.Name(myGrid.jqGrid('getCell', id, 'Name'));
            self.NameData(myGrid.jqGrid('getCell', id, 'NameData'));
            self.Description(myGrid.jqGrid('getCell', id, 'Description'));
            self.IsOrganisation(myGrid.jqGrid('getCell', id, 'IsOrganisation') == "true");
            self.IsDeleted(myGrid.jqGrid('getCell', id, 'IsDeleted') == "true");

            self.PositionEmployeeList.Init();

            self.IsRowSelected(true);
        }
    });

    self.Add = function (data, e) {
        self.PositionDetails.Init(self.OrganisationUID(), '', self.ReloadTree);
    };

    self.Remove = function (data, e) {
        app.Header.QuestionBox.InitQuestionBox("Вы уверены, что хотите архивировать должность?", function () {
            $.getJSON("/Positions/GetChildEmployeeUIDs/", { positionId: self.UID() }, function (employeeUIDs) {
                if (employeeUIDs.length > 0) {
                    app.Header.QuestionBox.InitQuestionBox("Существуют привязанные к должности сотрудники. Продолжить?", function() {
                        self.RemovePosition();
                    });
                } else {
                    self.RemovePosition();
                }
            })
            .fail(function (jqxhr, textStatus, error) {
                ShowError(jqxhr.responseText);
            });
        });
    };

    self.RemovePosition = function() {
            $.ajax({
                url: "Positions/MarkDeleted",
                type: "post",
                contentType: "application/json",
                data: JSON.stringify({ "uid": self.UID(), "name": self.Name() }),
                success: function () {
                    self.ReloadTree();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowError(xhr.responseText);
                }
            });
    };

    self.Edit = function (data, e) {
        self.PositionDetails.Init(self.OrganisationUID(), self.UID(), self.ReloadTree );
    };

    self.Copy = function (data, e) {
        self.Clipboard({ "Position": { "Name": self.NameData(), "Description": self.Description()}});
    };

    self.Paste = function (data, e) {
        self.Clipboard().Position.OrganisationUID = self.OrganisationUID();
        $.ajax({
            url: "Positions/PositionPaste",
            type: "post",
            contentType: "application/json",
            data: JSON.stringify({ "position": self.Clipboard() }),
            success: function (error) {
                    self.ReloadTree();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowError(xhr.responseText);
            }
        });
    };

    self.Restore = function (data, e) {
        app.Header.QuestionBox.InitQuestionBox("Вы уверены, что хотите восстановить должность?", function () {
            var ids = $("#jqGridPositions").getDataIDs();
            for (var i = 0; i < ids.length; i++) {
                var rowData = $("#jqGridPositions").getRowData(ids[i]);
                if (rowData.IsDeleted !== "true" &&
                    rowData.Name === self.Name() &&
                    rowData.OrganisationUID === self.OrganisationUID() &&
                    !rowData.IsOrganisation) {
                    alert("Существует неудалённый элемент с таким именем");
                    return;
                }
            }

            $.ajax({
                url: "Positions/Restore",
                type: "post",
                contentType: "application/json",
                data: JSON.stringify({ "uid": self.UID(), "name": self.Name() }),
                success: function () {
                    self.ReloadTree();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowError(xhr.responseText);
                }
            });
        });
    };

    return self;
}