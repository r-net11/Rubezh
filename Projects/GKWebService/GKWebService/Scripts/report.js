var Formatter = {
    link: function link(cellvalue, options, rowObject) {
        var link = cellvalue;
        if (options.colModel.formatoptions && options.colModel.formatoptions.urlColumnName) {
            link = rowObject[options.colModel.formatoptions.urlColumnName];
        }
        return "<a href='{0}' class='src-link' target='_blank'>{1}</a>".format(link, cellvalue);
    },
    utcDate: function utcDate(cellvalue, options, rowObject) {
        if (cellvalue) {
            var minuteOffset = (window.TimeZoneOffset / 60000) * -1;
            var date = moment.utc(cellvalue).zone(minuteOffset);
            return date.format(options.colModel.formatoptions.newformat);
        } else {
            return '';
        }
    },
    currentDate: function currentDate(cellvalue, options, rowObject) {
        if (cellvalue) {
            // format options
            var timeFormat;
            var dateFormat;
            if (options && options.colModel && options.colModel.formatoptions) {
                timeFormat = options.colModel.formatoptions.timeformat;
                dateFormat = options.colModel.formatoptions.dateformat;
            } else {
                timeFormat = 'hh:mm A';
                dateFormat = 'M/D/YYYY';
            }

            var minuteOffset = (window.TimeZoneOffset / 60000) * -1;
            var date = moment.utc(cellvalue).zone(minuteOffset);
            var now = moment.utc().zone(minuteOffset);
            var today = (date.year() == now.year() && date.dayOfYear() == now.dayOfYear());

            return date.format(today ? timeFormat : dateFormat);
        } else {
            return '';
        }
    },
    date: function date(cellvalue, options, rowObject) {
        return cellvalue ? moment(cellvalue).format(options.colModel.formatoptions.newformat) : "";
    },
    DateTZFormat: function (cellvalue, options, rowObject) {
        if (cellvalue) {
            return DateTimeFormatter(cellvalue, options.colModel.formatoptions.newformat || AppSettings.DateFormat);
        } else {
            return '';
        }
    },
    TimeTZFormat: function (cellvalue, options, rowObject) {
        if (cellvalue) {
            return DateTimeFormatter(cellvalue, options.colModel.formatoptions.newformat || AppSettings.TimeFormat);
        } else {
            return '';
        }
    },
    booleanText: function link(cellvalue, options, rowObject) {
        var text = cellvalue;
        if (options.colModel.formatoptions) {
            text = rowObject[options.colModel.name] ? options.colModel.formatoptions.trueText : options.colModel.formatoptions.falseText;
        }
        return text;
    },
    indicatorText: function link(cellvalue, options, rowObject) {
        // evgeny: todo: does it make sense to request boolean value for this field?
        var text = cellvalue;
        if (options.colModel.formatoptions) {
            text = rowObject[options.colModel.name] == "Y" ? options.colModel.formatoptions.trueText : options.colModel.formatoptions.falseText;
        }
        return text;
    },
    intArray: function intArray(cellvalue, options, rowObject) {
        var obj = $.grep(options.colModel.formatoptions, function (n, i) {
            return n.num == cellvalue;
        });
        return obj && obj.length ? obj[0].text : "";
    },
    references: function link(cellvalue, options, rowObject) {
        var result = "";
        if (cellvalue) {
            for (var i = 0; i < cellvalue.length; i++) {
                if (result) {
                    result += ", ";
                }
                result += Formatter.reference(cellvalue[i], options, rowObject);
            }
        }
        return result;
    },
    reference: function link(cellvalue, options, rowObject) {
        var nameField = "Name";
        if (options.colModel.formatoptions && options.colModel.formatoptions.nameField)
            nameField = options.colModel.formatoptions.nameField;
        cellvalue = cellvalue ? cellvalue[nameField] : "";
        return cellvalue ? cellvalue : "";
    }
};


$(document).ready(function () {

    jQuery.extend(jQuery.jgrid.defaults, {
        onSelectAll: function (ids, selected) {
            $(this).triggerHandler("selectAll.jqGrid", [ids, selected]);
        },
        onSelectRow: function (id, selected) {
            $(this).triggerHandler("selectRow.jqGrid", [id, selected]);
        }
    });

    $("#jqGrid").jqGrid({
        url: '/Home/GetReports',
        datatype: "json",
        colModel: [
           { label: 'Дата в приборе', name: 'DeviceDate', formatter: Formatter.date, formatoptions: { newformat: 'M/D/YYYY HH:mm:ss' }, width: 75, sortable: false },
           { label: 'Дата в системе', name: 'SystemDate', formatter: Formatter.date, formatoptions: { newformat: 'M/D/YYYY HH:mm:ss' }, width: 90, sortable: false },
           { label: 'Название', name: 'Name', width: 100, sortable: false },
           { label: 'Уточнение', name: 'Desc', width: 80, sortable: false },
           { label: 'Объект', name: 'Object', width: 80, sortable: false }
        ],
        width: 900,
        height: 300,
        rowNum: 100,
        viewrecords: true,
        pager: "#jqGridPager"
    });

    
});



function ReportViewModel() {
    var self = {};

    self.DeviceDate = ko.observable();
    self.SystemDate = ko.observable();
    self.Name = ko.observable();
    self.Desc = ko.observable();

    $('#jqGrid').on('jqGridSelectRow', function (event, id, selected) {

        var myGrid = $('#jqGrid');

        self.DeviceDate(myGrid.jqGrid('getCell', id, 'DeviceDate'));
        self.SystemDate(myGrid.jqGrid('getCell', id, 'SystemDate'));
        self.Name(myGrid.jqGrid('getCell', id, 'Name'));
        self.Desc(myGrid.jqGrid('getCell', id, 'Desc'));
    });

    
    

    return self;
}
