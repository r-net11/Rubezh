function InitGridCards() {
    function imageFormat(cellvalue, options, rowObject) {
        var newCellValue = cellvalue;
        if (rowObject.IsDeleted) {
            newCellValue = '<span style="opacity:0.5">' + newCellValue + '</span>';
        };
        if (rowObject.IsOrganisation) {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Organisation.png" />' + newCellValue;
        } 
        else if (rowObject.IsDeactivatedRootItem) {
            return '<img style="vertical-align: middle; padding-right: 3px" width="16" height="16" src="/Content/Image/Icon/Hr/Lock.png" />' + newCellValue;
        }
        else {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Card.png" />' + newCellValue;
        }
};

    $("#jqGridCards").jqGrid({
        datastr: null,
        datatype: "jsonstring",
        colModel: [
            { label: 'UID', name: 'UID', key: true, hidden: true, sortable: false },
            { label: 'ParentUID', name: 'ParentUID', hidden: true, sortable: false },
            { label: 'OrganisationUID', name: 'OrganisationUID', hidden: true, sortable: false },
            { label: 'IsOrganisation', name: 'IsOrganisation', hidden: true, sortable: false },
            { label: 'Номер', name: 'Number', width: 100, hidden: false, sortable: false, formatter: imageFormat },
            { label: 'Тип', name: 'CardType', hidden: false, sortable: false },
            { label: 'Сотрудник', name: 'EmployeeName', hidden: false, sortable: false },
            { label: 'Причина деактивации', name: 'StopReason', hidden: false, sortable: false },
            { label: 'IsDeleted', name: 'IsDeleted', hidden: true, sortable: false },
            { label: 'IsDeactivatedRootItem', name: 'IsDeactivatedRootItem', hidden: true, sortable: false },
            { label: 'IsInStopList', name: 'IsInStopList', hidden: true, sortable: false }
        ],
        width: jQuery(window).width() - 242,
        height: 500,
        rowNum: 100,
        viewrecords: true,

        treeGrid: true,
        ExpandColumn: "Number",
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

function CardsViewModel() {
    var self = {};

    InitGridCards();

    self.UID = ko.observable();
    self.ParentUID = ko.observable();
    self.OrganisationUID = ko.observable();
    self.IsOrganisation = ko.observable(true);
    self.IsRowSelected = ko.observable(false);
    self.IsDeleted = ko.observable();
    self.IsDeactivatedRootItem = ko.observable(false);
    self.IsInStopList = ko.observable(false);

    self.CanRemove = ko.computed(function () {
        return self.IsRowSelected() && self.IsInStopList() && !self.IsOrganisation() && !self.IsDeactivatedRootItem() && app.Menu.HR.IsCardsEditAllowed();
    }, self);

    self.Init = function (filter) {
        self.Filter = filter;
        self.ReloadTree();
    };

    self.ReloadTree = function () {
        self.IsOrganisation(true);
        self.IsRowSelected(false);

        $.ajax({
            url: "/Cards/GetOrganisations",
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
        $("#jqGridCards").setGridParam({
            datastr: data,
            datatype: "jsonstring",
            treedatatype: "jsonstring",
        });
        $("#jqGridCards").trigger("reloadGrid");
        $("#jqGridCards").jqGrid("resetSelection");
    };

    $('#jqGridCards').on('jqGridSelectRow', function (event, id, selected) {

        if (selected) {
            var myGrid = $('#jqGridCards');

            self.UID(id);
            self.OrganisationUID(myGrid.jqGrid('getCell', id, 'OrganisationUID'));
            self.ParentUID(myGrid.jqGrid('getCell', id, 'ParentUID'));
            self.IsOrganisation(myGrid.jqGrid('getCell', id, 'IsOrganisation') == "true");
            self.IsDeleted(myGrid.jqGrid('getCell', id, 'IsDeleted') == "true");
            self.IsDeactivatedRootItem(myGrid.jqGrid('getCell', id, 'IsDeactivatedRootItem') == "true");
            self.IsInStopList(myGrid.jqGrid('getCell', id, 'IsInStopList') == "true");

            self.IsRowSelected(true);
        }
    });

    self.Remove = function (data, e) {
        app.Header.QuestionBox.InitQuestionBox("Вы уверены, что хотите удалить карту?", function () {
            self.RemoveCard();
        });
    };

    self.RemoveCard = function() {
            $.ajax({
                url: "Cards/MarkDeleted",
                type: "post",
                contentType: "application/json",
                data: JSON.stringify({ "uid": self.UID()}),
                success: function () {
                    self.ReloadTree();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowError(xhr.responseText);
                }
            });
    };

    return self;
}