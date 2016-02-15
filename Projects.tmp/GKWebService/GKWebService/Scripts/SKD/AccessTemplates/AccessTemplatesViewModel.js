function InitGridAccessTemplates() {
    function imageFormat(cellvalue, options, rowObject) {
        var newCellValue = cellvalue;
        if (rowObject.IsDeleted) {
            newCellValue = '<span style="opacity:0.5">' + newCellValue + '</span>';
        };
        if (rowObject.IsOrganisation) {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Organisation.png" />' + newCellValue;
        } else {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/AccessTemplate.png" />' + newCellValue;
        }
    };

    $("#jqGridAccessTemplates").jqGrid({
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

function InitGridAccessTemplateDoors() {
    $("#jqGridAccessTemplateDoors").jqGrid({
        datastr: null,
        datatype: "jsonstring",
        colModel: [
            { label: 'Точка доступа', name: 'PresentationName', sortable: false },
            { label: 'Вход', name: 'EnerScheduleName', sortable: false },
            { label: 'Выход', name: 'ExitScheduleName', sortable: false }
        ],
        width: jQuery(window).width() - 242,
        height: 200,
        rowNum: 100,
        viewrecords: true
    });
};

function AccessTemplatesViewModel() {
    var self = {};

    InitGridAccessTemplates();
    InitGridAccessTemplateDoors();

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
        return self.IsRowSelected() && !self.IsDeleted() && app.Menu.HR.IsAccessTemplatesEditAllowed();
    }, self);
    self.CanRemove = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && !self.IsOrganisation() && app.Menu.HR.IsAccessTemplatesEditAllowed();
    }, self);
    self.CanEdit = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && !self.IsOrganisation() && app.Menu.HR.IsAccessTemplatesEditAllowed();
    }, self);
    self.CanCopy = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && !self.IsOrganisation() && app.Menu.HR.IsAccessTemplatesEditAllowed();
    }, self);
    self.CanPaste = ko.computed(function () {
        return self.Clipboard() && self.OrganisationUID() === self.Clipboard().AccessTemplate.OrganisationUID && self.IsRowSelected() && !self.IsDeleted() && self.Clipboard() && app.Menu.HR.IsAccessTemplatesEditAllowed();
    }, self);
    self.CanRestore = ko.computed(function () {
        return self.IsRowSelected() && self.IsDeleted() && !self.IsOrganisation() && app.Menu.HR.IsAccessTemplatesEditAllowed();
    }, self);

    self.Init = function (filter) {
        self.Filter = filter;
        self.Clipboard(null);
        self.ReloadTree();
    };

    self.ReloadTree = function () {
        self.IsOrganisation(true);
        self.IsRowSelected(false);
        self.ReloadDoors();

        $.ajax({
            url: "/AccessTemplates/GetOrganisations",
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
        $("#jqGridAccessTemplates").setGridParam({
            datastr: data,
            datatype: "jsonstring",
            treedatatype: "jsonstring",
        });
        $("#jqGridAccessTemplates").trigger("reloadGrid");
        $("#jqGridAccessTemplates").jqGrid("resetSelection");
    };

    $('#jqGridAccessTemplates').on('jqGridSelectRow', function (event, id, selected) {

        if (selected) {
            var myGrid = $('#jqGridAccessTemplates');

            self.UID(id);
            self.OrganisationUID(myGrid.jqGrid('getCell', id, 'OrganisationUID'));
            self.ParentUID(myGrid.jqGrid('getCell', id, 'ParentUID'));
            self.Name(myGrid.jqGrid('getCell', id, 'Name'));
            self.NameData(myGrid.jqGrid('getCell', id, 'NameData'));
            self.Description(myGrid.jqGrid('getCell', id, 'Description'));
            self.IsOrganisation(myGrid.jqGrid('getCell', id, 'IsOrganisation') == "true");
            self.IsDeleted(myGrid.jqGrid('getCell', id, 'IsDeleted') == "true");

            self.ReloadDoors();

            self.IsRowSelected(true);
        }
    });

    self.ReloadDoors = function () {
        if (self.IsOrganisation()) {
            self.UpdateDoors([]);
        } else
        {
            $.getJSON("AccessTemplates/GetDoors/" + self.UID(), function (doors) {
                self.UpdateDoors(doors);
            })
            .fail(function (jqxhr, textStatus, error) {
                ShowError(jqxhr.responseText);
            });
        }
    };

    self.UpdateDoors = function (data) {
        $("#jqGridAccessTemplateDoors").setGridParam({
            datastr: ko.toJSON(data),
            datatype: "jsonstring",
        });
        $("#jqGridAccessTemplateDoors").trigger("reloadGrid");
    };

    self.Add = function (data, e) {
        self.AccessTemplateDetails.Init(self.OrganisationUID(), '', self.ReloadTree);
    };

    self.Remove = function (data, e) {
        app.Header.QuestionBox.InitQuestionBox("Вы уверены, что хотите архивировать шаблон доступа?", function () {
            $.getJSON("AccessTemplates/GetLinkedCards",
                { organisationId: self.OrganisationUID(), id: self.UID() },
                function (numbersSting) {
                    if (numbersSting) {
                        app.Header.QuestionBox.InitQuestionBox("Шаблон привязан к пропускам номер " + numbersSting + ". При удалении шаблона указанные в нём точки доступа будут убраны из привязаных пропусков. Вы уверены, что хотите удалить шаблон?", function() {
                            self.RemoveAccessTemplate();
                    });
                } else {
                    self.RemoveAccessTemplate();
                }
            })
            .fail(function (jqxhr, textStatus, error) {
                ShowError(jqxhr.responseText);
            });
        });
    };

    self.RemoveAccessTemplate = function () {
            $.ajax({
                url: "AccessTemplates/MarkDeleted",
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
        self.AccessTemplateDetails.Init(self.OrganisationUID(), self.UID(), self.ReloadTree );
    };

    self.Copy = function (data, e) {
        $.getJSON("/AccessTemplates/GetAccessTemplateDetails/",
            { organisationId: self.OrganisationUID(), id: self.UID() },
            function (data) {
                self.Clipboard(data);
            })
        .fail(function (jqxhr, textStatus, error) {
            ShowError(jqxhr.responseText);
        });
    };

    self.Paste = function (data, e) {
        self.Clipboard().AccessTemplate.OrganisationUID = self.OrganisationUID();
        $.ajax({
            url: "AccessTemplates/AccessTemplatePaste",
            type: "post",
            contentType: "application/json",
            data: JSON.stringify({ "accessTemplate": self.Clipboard() }),
            success: function (error) {
                    self.ReloadTree();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowError(xhr.responseText);
            }
        });
    };

    self.Restore = function (data, e) {
        app.Header.QuestionBox.InitQuestionBox("Вы уверены, что хотите восстановить шаблон доступа?", function () {
            var ids = $("#jqGridAccessTemplates").getDataIDs();
            for (var i = 0; i < ids.length; i++) {
                var rowData = $("#jqGridAccessTemplates").getRowData(ids[i]);
                if (rowData.IsDeleted !== "true" &&
                    rowData.Name === self.Name() &&
                    rowData.OrganisationUID === self.OrganisationUID() &&
                    !rowData.IsOrganisation) {
                    alert("Существует неудалённый элемент с таким именем");
                    return;
                }
            }

            $.ajax({
                url: "AccessTemplates/Restore",
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