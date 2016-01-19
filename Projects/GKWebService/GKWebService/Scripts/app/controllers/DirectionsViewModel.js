$(document).ready(function () {

    function imageFormat(cellvalue, options, rowObject) {

        //return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/' + rowObject.StateIcon + '.png" />' + rowObject.State;

        if (rowObject.State == 'Отсутствует лицензия') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/HasNoLicense.png" />' + rowObject.State;
        }

        if (rowObject.State == 'База данных прибора не соответствует базе данных ПК') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/DBMissmatch.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Контроллер в технологическом режиме') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/TechnologicalRegime.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Потеря связи') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/ConnectionLost.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Пожар 2') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/Fire2.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Пожар 1') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/Fire1.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Внимание') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/Attention.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Отключено') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/Ignore.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Неисправность') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/Failure.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Включено') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/On.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Включается') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/TurningOn.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Выключается') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/TurningOff.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Автоматика отключена') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/AutoOff.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Требуется обслуживание') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/Service.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Тест') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/Test.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Информация') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/Info.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Выключено') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/Off.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Неизвестно') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/Unknown.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Норма') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/Norm.png" />' + rowObject.State;
        }

        if (rowObject.State == 'Нет') {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/No.png" />' + rowObject.State;
        }
    };

    $("#jqGridDirections").jqGrid({
        url: '/Home/GetDirections',
        datatype: "json",
        colModel: [
            { label: 'UID', name: 'UID', key: true, hidden: true, sortable: false },
            { label: 'StateIcon', name: 'StateIcon', key: true, hidden: true, sortable: false },
            { label: 'Номер', name: 'No', key: true, hidden: false, sortable: false },
            { label: 'Наименование', name: 'Name', hidden: false, sortable: false },
            { label: 'Состояние', name: 'State', hidden: false, sortable: false, formatter: imageFormat },
        ],
        width: jQuery(window).width() - 300,
        height: jQuery(window).height() - 10,
        rowNum: 100,
        viewrecords: true
    });

});

function DirectionsViewModel() {
    var self = {};

    self.No = ko.observable();
    self.Name = ko.observable();

    $('#jqGridDirections').on('jqGridSelectRow', function (event, id, selected) {

        var myGrid = $('#jqgrid');

        self.No(myGrid.jqGrid('getCell', id, 'No'));
        self.Name(myGrid.jqGrid('getCell', id, 'Name'))

    });

    return self;
}

