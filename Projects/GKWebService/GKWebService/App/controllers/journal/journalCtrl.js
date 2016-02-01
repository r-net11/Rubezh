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

(function () {
	'use strict';
	angular.module('canvasApp.controllers').controller('journalCtrl', function ($scope, $http, $uibModal, uiGridConstants, signalrJournalService) {
		$http.get("Journal/GetJournal").success(function (data) {
			$scope.gridOptions.data = data;
		});
		
		$scope.gridOptions = {
			enableRowSelection: true,
			enableRowHeaderSelection: false,
			multiSelect: false,
			modifierKeysToMultiSelect: true,
			noUnselect: true,
			enableSorting: false,
			enableColumnResizing: true,
			enableColumnMenus: false,
			onRegisterApi: function(gridApi) {
				$scope.gridApi = gridApi;
				gridApi.selection.on.rowSelectionChanged($scope, $scope.showSelectedRow);
				gridApi.selection.on.rowSelectionChangedBatch($scope, $scope.showSelectedRow);
			},
			columnDefs: [
				{ name: 'Дата в системе', field: 'SystemDate' },
				{ name: 'Дата в приборе', field: 'DeviceDate' },
				{ name: 'Название', field: 'Name' },
				{ name: 'Уточнение', field: 'Desc' },
				{ name: 'Объект', field: 'Object' },
				{
					name: 'Подсистема',
					cellTemplate:
						'<div class="ui-grid-cell-contents">\
							<img style="vertical-align: middle; padding-right: 3px; width: 16px" ng-src="/Content/Image/Icon/SubsystemTypes/{{row.entity.SubsystemImage}}.png" />\
							{{row.entity.Subsystem}}\
						</div>'
				}
			]
		};

		$scope.showFilter = function () {
			var modalInstance = $uibModal.open({
				animation: false,
				templateUrl: 'Journal/JournalFilter',
			});
		};

		$scope.showSelectedRow = function () {
			$scope.selectedRow = $scope.gridApi.selection.getSelectedRows()[0]
		};

		$scope.$on('updateJournalItemsJs', function (event, args) {
			args.forEach(function (element) {
				$scope.gridOptions.data.unshift(element)
			});
			$scope.$apply();
		})
	});
}());
