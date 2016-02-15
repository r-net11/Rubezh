$(function () {
    var ticker = $.connection.directionsUpdater;

    function init() {
    }

    $.extend(ticker.client, {
        updateDirection: function (direction) {
            var directionRowID = direction.UID;
            var rowData = $('#jqGridDirections').jqGrid('getRowData', directionRowID);
            rowData.State = direction.State;
            rowData.StateIcon = direction.StateIcon;
            rowData.No = direction.No;
            $('#jqGridDirections').jqGrid('setRowData', directionRowID, rowData);
        },

    });

    $.connection.hub.start()
        .then(init);
});