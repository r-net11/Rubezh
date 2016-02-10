
$(document).ready(function () {
    $("#jqGridArchive").jqGrid({
        url: '/Journal/GetJournal',
        datatype: "json",
        colModel: [
           { label: 'Дата в приборе', name: 'DeviceDate', formatter: Formatter.date, formatoptions: { newformat: 'M/D/YYYY HH:mm:ss' }, width: 75, sortable: false },
           { label: 'Дата в системе', name: 'SystemDate', formatter: Formatter.date, formatoptions: { newformat: 'M/D/YYYY HH:mm:ss' }, width: 90, sortable: false },
           { label: 'Название', name: 'Name', width: 100, sortable: false },
           { label: 'Уточнение', name: 'Desc', width: 80, sortable: false },
           { label: 'Объект', name: 'Object', width: 80, sortable: false }
        ],
        width: jQuery(window).width() - 242,
        height: 300,
        rowNum: 100,
        viewrecords: true,
        pager: "#jqGridArchivePager"
    });
});

function ArchiveViewModel() {
    var self = {};
    self.DeviceDate = ko.observable();
    self.SystemDate = ko.observable();
    self.Name = ko.observable();
    self.Desc = ko.observable();
    $('#jqGridArchive').on('jqGridSelectRow', function (event, id, selected) {
        var myGrid = $('#jqGrid');
        self.DeviceDate(myGrid.jqGrid('getCell', id, 'DeviceDate'));
        self.SystemDate(myGrid.jqGrid('getCell', id, 'SystemDate'));
        self.Name(myGrid.jqGrid('getCell', id, 'Name'));
        self.Desc(myGrid.jqGrid('getCell', id, 'Desc'));
    });
    return self;
}
