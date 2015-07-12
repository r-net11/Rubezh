$(document).ready(function () {

    $("#jqGrid").jqGrid({
        url: '/Home/GetReports/',
        datatype: "json",
        colModel: [
           { label: 'Дата в приборе', name: 'DeviceDate', width: 75, sortable: false },
           { label: 'Дата в системе', name: 'SystemDate', width: 90, sortable: false },
           { label: 'Название', name: 'Name', width: 100, sortable: false },
           { label: 'Уточнение', name: 'Desc', width: 80, sortable: false },
           { label: 'Объект', name: 'Object', width: 80, sortable: false }
        ],
        //viewrecords: true, // show the current page, data rang and total records on the toolbar
        width: 900,
        height: 400,
        rowNum: 30,
        pager: "#jqGridPager"
    });
});