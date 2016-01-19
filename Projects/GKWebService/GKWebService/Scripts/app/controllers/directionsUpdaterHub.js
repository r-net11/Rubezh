if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                var r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}

$(function () {

    var ticker = $.connection.directionsUpdater, // the generated client-side hub proxy
        $directionsTable = $('#directionsTable'),
        $directionsTableBody = $directionsTable.find('tbody'),
        rowTemplate = '<tr data-symbol="{UID}"><td>{No}</td><td>{Name}</td><td>{State}</td></tr>';

    function init() {
        return ticker.server.getAllDirections().done(function (directions) {
            $.each(directions, function () {
                $directionsTableBody.append(rowTemplate.supplant(this));
            });
        });
    }

    // Add client-side hub methods that the server will call
    $.extend(ticker.client, {
        updateDirection: function (direction) {
            var $row = $(rowTemplate.supplant(direction));

            $directionsTableBody.find('tr[data-symbol=' + direction.UID + ']')
                .replaceWith($row);
        },

    });

    // Start the connection
    $.connection.hub.start()
        .then(init);
});